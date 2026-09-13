using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PSC09
{
    // Una línea de una orden de compra (artículo, cantidad y costo unitario pactado).
    public class LineaOrdenCompra
    {
        public string Articulo;
        public string Descripcion;
        public decimal Cantidad;
        public decimal CostoUnitario;
    }

    // Guardado, recepción y anulación de una Orden de Compra (Registro → Órdenes de
    // Compra, frmOrdenCompra). Mismo patrón de transacción única que
    // FacturaService.GuardarFactura/AnularFactura: o se completa todo, o no cambia nada.
    public static class OrdenCompraService
    {
        public const string EstadoPendiente = "Pendiente";
        public const string EstadoRecibida = "Recibida";
        public const string EstadoAnulada = "Anulada";

        // Métodos de costeo al recibir una orden (ver RecibirOrden): "varias formas" de
        // actualizar PRODUCTOS.costo, a elegir por el usuario en cada recepción.
        public const string CosteoNinguno = "Ninguno";
        public const string CosteoUltimoCosto = "UltimoCosto";
        public const string CosteoPromedioPonderado = "PromedioPonderado";

        // Inserta la orden en estado Pendiente (todavía no toca inventario ni Cuentas
        // por Pagar; eso ocurre recién al RecibirOrden). Si numero viene vacío, asigna
        // uno nuevo aquí mismo. Devuelve el número usado.
        public static string GuardarOrden(string numero, DateTime fecha, int idProveedor, string nota, List<LineaOrdenCompra> lineas)
        {
            if (lineas == null || lineas.Count == 0)
            {
                throw new Exception("Agrega al menos un artículo antes de guardar.");
            }

            if (string.IsNullOrWhiteSpace(numero))
            {
                numero = Busco.BuscaUltimoNumero("4");
            }

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                using (SqlTransaction tx = cnx.BeginTransaction())
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand(
                            " INSERT INTO ORDENCOMPRA (NUMERO, FECHA, IDPROVEEDOR, ESTADO, NOTA, ACTIVO) " +
                            " VALUES (@numero, @fecha, @idProveedor, @estado, @nota, 1) ", cnx, tx);
                        cmd.Parameters.AddWithValue("@numero", numero);
                        cmd.Parameters.AddWithValue("@fecha", fecha.ToString("dd/MM/yyyy"));
                        cmd.Parameters.AddWithValue("@idProveedor", idProveedor);
                        cmd.Parameters.AddWithValue("@estado", EstadoPendiente);
                        cmd.Parameters.AddWithValue("@nota", (object)nota ?? DBNull.Value);
                        cmd.ExecuteNonQuery();

                        SqlCommand cmdSec = new SqlCommand("UPDATE SECUENCIA SET SECUENCIA = @numero WHERE id = 4", cnx, tx);
                        cmdSec.Parameters.AddWithValue("@numero", numero);
                        cmdSec.ExecuteNonQuery();

                        foreach (LineaOrdenCompra linea in lineas)
                        {
                            SqlCommand cmdDet = new SqlCommand(
                                " INSERT INTO DORDENCOMPRA (ORDENCOMPRA, ARTICULO, CANTIDAD, COSTOUNITARIO, ACTIVO) " +
                                " VALUES (@orden, @articulo, @cantidad, @costo, 1) ", cnx, tx);
                            cmdDet.Parameters.AddWithValue("@orden", numero);
                            cmdDet.Parameters.AddWithValue("@articulo", linea.Articulo);
                            cmdDet.Parameters.AddWithValue("@cantidad", linea.Cantidad);
                            cmdDet.Parameters.AddWithValue("@costo", linea.CostoUnitario);
                            cmdDet.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }

            return numero;
        }

        // Marca la orden como Recibida: por cada línea genera la entrada de inventario
        // (InventarioService, que ya actualiza PRODUCTOS.cantidad), actualiza
        // PRODUCTOS.costo según metodoCosteo, y registra el cargo en Cuentas por Pagar
        // por el total de la orden (cantidad × costo de cada línea sumado). Todo en una
        // sola transacción: si algo falla, la orden sigue Pendiente y nada de esto pasó.
        public static void RecibirOrden(string numeroOrden, string metodoCosteo)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                using (SqlTransaction tx = cnx.BeginTransaction())
                {
                    try
                    {
                        int idProveedor;
                        DateTime fecha;
                        string estadoActual;

                        SqlCommand cmdOrden = new SqlCommand(
                            "SELECT IDPROVEEDOR, FECHA, ESTADO FROM ORDENCOMPRA WHERE NUMERO = @numero AND ACTIVO = 1", cnx, tx);
                        cmdOrden.Parameters.AddWithValue("@numero", numeroOrden);
                        using (SqlDataReader rdr = cmdOrden.ExecuteReader())
                        {
                            if (!rdr.Read())
                            {
                                throw new Exception("No se encontró la orden de compra " + numeroOrden + ".");
                            }
                            idProveedor = Convert.ToInt32(rdr["IDPROVEEDOR"]);
                            fecha = DateTime.ParseExact(Convert.ToString(rdr["FECHA"]), "dd/MM/yyyy", null);
                            estadoActual = Convert.ToString(rdr["ESTADO"]);
                        }

                        if (estadoActual != EstadoPendiente)
                        {
                            throw new Exception("Esta orden ya está " + estadoActual.ToLower() + "; sólo se puede recibir una orden Pendiente.");
                        }

                        List<LineaOrdenCompra> lineas = new List<LineaOrdenCompra>();
                        SqlCommand cmdLineas = new SqlCommand(
                            "SELECT ARTICULO, CANTIDAD, COSTOUNITARIO FROM DORDENCOMPRA WHERE ORDENCOMPRA = @numero AND ACTIVO = 1", cnx, tx);
                        cmdLineas.Parameters.AddWithValue("@numero", numeroOrden);
                        using (SqlDataReader rdr = cmdLineas.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                lineas.Add(new LineaOrdenCompra
                                {
                                    Articulo = Convert.ToString(rdr["ARTICULO"]),
                                    Cantidad = Convert.ToDecimal(rdr["CANTIDAD"]),
                                    CostoUnitario = Convert.ToDecimal(rdr["COSTOUNITARIO"])
                                });
                            }
                        }

                        decimal totalOrden = 0;
                        foreach (LineaOrdenCompra linea in lineas)
                        {
                            totalOrden += linea.Cantidad * linea.CostoUnitario;

                            // Cantidad/costo ANTES de que InventarioService sume la entrada, para
                            // poder calcular el promedio ponderado sobre la existencia previa.
                            decimal cantidadPrevia = 0, costoPrevio = 0;
                            SqlCommand cmdProd = new SqlCommand("SELECT CANTIDAD, COSTO FROM PRODUCTOS WHERE ITEM = @item", cnx, tx);
                            cmdProd.Parameters.AddWithValue("@item", linea.Articulo);
                            using (SqlDataReader rdr = cmdProd.ExecuteReader())
                            {
                                if (rdr.Read())
                                {
                                    cantidadPrevia = rdr["CANTIDAD"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["CANTIDAD"]);
                                    costoPrevio = rdr["COSTO"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["COSTO"]);
                                }
                            }

                            InventarioService.RegistrarMovimiento(cnx, tx, linea.Articulo, fecha, InventarioService.Entrada, linea.Cantidad, "OrdenCompra", numeroOrden, null);

                            decimal? nuevoCosto = null;
                            if (metodoCosteo == CosteoUltimoCosto)
                            {
                                nuevoCosto = linea.CostoUnitario;
                            }
                            else if (metodoCosteo == CosteoPromedioPonderado)
                            {
                                decimal cantidadTotal = cantidadPrevia + linea.Cantidad;
                                nuevoCosto = cantidadTotal > 0
                                    ? Math.Round((cantidadPrevia * costoPrevio + linea.Cantidad * linea.CostoUnitario) / cantidadTotal, 2)
                                    : linea.CostoUnitario;
                            }

                            if (nuevoCosto.HasValue)
                            {
                                SqlCommand cmdCosto = new SqlCommand("UPDATE PRODUCTOS SET COSTO = @costo WHERE ITEM = @item", cnx, tx);
                                cmdCosto.Parameters.AddWithValue("@costo", nuevoCosto.Value);
                                cmdCosto.Parameters.AddWithValue("@item", linea.Articulo);
                                cmdCosto.ExecuteNonQuery();
                            }
                        }

                        SqlCommand cmdEstado = new SqlCommand(
                            "UPDATE ORDENCOMPRA SET ESTADO = @estado, METODOCOSTEO = @metodo WHERE NUMERO = @numero", cnx, tx);
                        cmdEstado.Parameters.AddWithValue("@estado", EstadoRecibida);
                        cmdEstado.Parameters.AddWithValue("@metodo", metodoCosteo);
                        cmdEstado.Parameters.AddWithValue("@numero", numeroOrden);
                        cmdEstado.ExecuteNonQuery();

                        CuentaProveedor.RegistrarCargo(cnx, tx, idProveedor, fecha, numeroOrden, totalOrden);

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        // Anula una orden (baja lógica, no física). Si ya estaba Recibida, revierte la
        // entrada de inventario de cada línea y el cargo en Cuentas por Pagar; si
        // todavía estaba Pendiente, sólo se marca anulada porque nunca llegó a tocar
        // inventario ni cuentas por pagar.
        public static void AnularOrden(string numeroOrden)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                using (SqlTransaction tx = cnx.BeginTransaction())
                {
                    try
                    {
                        string estadoActual;
                        SqlCommand cmdOrden = new SqlCommand("SELECT ESTADO FROM ORDENCOMPRA WHERE NUMERO = @numero AND ACTIVO = 1", cnx, tx);
                        cmdOrden.Parameters.AddWithValue("@numero", numeroOrden);
                        object resultado = cmdOrden.ExecuteScalar();
                        if (resultado == null)
                        {
                            throw new Exception("No se encontró la orden de compra " + numeroOrden + ".");
                        }
                        estadoActual = Convert.ToString(resultado);

                        if (estadoActual == EstadoRecibida)
                        {
                            List<Tuple<string, decimal>> lineas = new List<Tuple<string, decimal>>();
                            SqlCommand cmdLineas = new SqlCommand(
                                "SELECT ARTICULO, CANTIDAD FROM DORDENCOMPRA WHERE ORDENCOMPRA = @numero AND ACTIVO = 1", cnx, tx);
                            cmdLineas.Parameters.AddWithValue("@numero", numeroOrden);
                            using (SqlDataReader rdr = cmdLineas.ExecuteReader())
                            {
                                while (rdr.Read())
                                {
                                    lineas.Add(Tuple.Create(Convert.ToString(rdr["ARTICULO"]), Convert.ToDecimal(rdr["CANTIDAD"])));
                                }
                            }

                            foreach (Tuple<string, decimal> linea in lineas)
                            {
                                InventarioService.RegistrarMovimiento(cnx, tx, linea.Item1, DateTime.Now, InventarioService.Salida, linea.Item2, "AnulacionOrdenCompra", numeroOrden, null);
                            }

                            CuentaProveedor.AnularCargosDeOrden(cnx, tx, numeroOrden);
                        }

                        SqlCommand cmdEstado = new SqlCommand("UPDATE ORDENCOMPRA SET ESTADO = @estado, ACTIVO = 0 WHERE NUMERO = @numero", cnx, tx);
                        cmdEstado.Parameters.AddWithValue("@estado", EstadoAnulada);
                        cmdEstado.Parameters.AddWithValue("@numero", numeroOrden);
                        cmdEstado.ExecuteNonQuery();

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        // Líneas de una orden (con descripción del artículo, vía JOIN), para mostrarla
        // en frmOrdenCompra al reabrirla.
        public static List<LineaOrdenCompra> ObtenerLineas(string numeroOrden)
        {
            List<LineaOrdenCompra> lista = new List<LineaOrdenCompra>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT D.ARTICULO, P.DESCRIPCION, D.CANTIDAD, D.COSTOUNITARIO " +
                    " FROM DORDENCOMPRA D INNER JOIN PRODUCTOS P ON D.ARTICULO = P.ITEM " +
                    " WHERE D.ORDENCOMPRA = @numero AND D.ACTIVO = 1", cnx);
                cmd.Parameters.AddWithValue("@numero", numeroOrden);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new LineaOrdenCompra
                        {
                            Articulo = Convert.ToString(rdr["ARTICULO"]),
                            Descripcion = Convert.ToString(rdr["DESCRIPCION"]),
                            Cantidad = Convert.ToDecimal(rdr["CANTIDAD"]),
                            CostoUnitario = Convert.ToDecimal(rdr["COSTOUNITARIO"])
                        });
                    }
                }
            }

            return lista;
        }
    }
}
