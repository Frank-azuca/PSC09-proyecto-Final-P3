using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PSC09
{
    // Un movimiento de la cuenta de un cliente (cargo por factura o abono/pago),
    // tal como se muestra en frmEstadoCuenta.
    public class MovimientoCuenta
    {
        public string Fecha;
        public string Documento;
        public decimal Monto;
        public decimal SaldoDespues;
        public bool EsAbono;
    }

    public class TipoPago
    {
        public int Id;
        public string Nombre;

        public override string ToString()
        {
            return Nombre;
        }
    }

    // Una línea de pago dentro de un recibo (parte del monto pagada con una forma de
    // pago específica: Efectivo, Tarjeta, etc.). Un recibo puede repartirse entre
    // varias, por ejemplo una venta pagada mitad en efectivo y mitad con tarjeta.
    public class LineaPago
    {
        public int IdTipoPago;
        public string NombreTipoPago;
        public decimal Monto;
    }

    // Cuentas por cobrar por cliente (MUTOCTE): cada factura genera un cargo, cada
    // anulación lo revierte, y un recibo de ingreso (uno o varios pagos, cada uno con
    // su forma de pago) reduce lo que el cliente debe. bcPendiente guarda el saldo tal
    // como quedó justo después de ese movimiento (una fotografía, no una fórmula), así
    // que frmEstadoCuenta siempre recalcula el saldo actual con un SUM en vivo en vez
    // de confiar en el bcPendiente de la última fila: si un cargo viejo se desactiva
    // (factura anulada) después de que ya hubo movimientos posteriores, esas
    // fotografías anteriores quedan "desactualizadas" a propósito (son historial),
    // pero el saldo que se muestra siempre es el real.
    public static class CuentaCliente
    {
        public const int OrigenCargo = 1;
        public const int OrigenAbono = 2;

        // Se llama dentro de la misma transacción de FacturaService.GuardarFactura():
        // si la factura no se guarda, el cargo tampoco queda. Si el cliente no es un
        // número válido (factura sin cliente), no registra nada.
        public static void RegistrarCargo(SqlConnection cnx, SqlTransaction tx, string idClienteTexto, DateTime fecha, string documento, decimal monto)
        {
            RegistrarMovimiento(cnx, tx, idClienteTexto, fecha, OrigenCargo, documento, monto);
        }

        // Se llama dentro de la misma transacción de FacturaService.AnularFactura():
        // desactiva el cargo de esa factura (no lo borra), igual que el resto del
        // sistema anula en vez de eliminar.
        public static void AnularCargosDeFactura(SqlConnection cnx, SqlTransaction tx, string numeroFactura)
        {
            SqlCommand cmd = new SqlCommand(
                "UPDATE MUTOCTE SET ACTIVO = 0 WHERE DOCUMENTO = @doc AND ORIGEN = @origen AND ACTIVO = 1", cnx, tx);
            cmd.Parameters.AddWithValue("@doc", numeroFactura);
            cmd.Parameters.AddWithValue("@origen", OrigenCargo);
            cmd.ExecuteNonQuery();
        }

        public static List<TipoPago> ObtenerTiposPago(bool soloActivos = true)
        {
            List<TipoPago> lista = new List<TipoPago>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT ID, NOMBRE FROM TIPOPAGO " + (soloActivos ? " WHERE ACTIVO = 1 " : "") + " ORDER BY ID", cnx);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new TipoPago { Id = Convert.ToInt32(rdr["ID"]), Nombre = Convert.ToString(rdr["NOMBRE"]) });
                    }
                }
            }

            return lista;
        }

        // Registra un recibo de ingreso: uno o varios pagos (lineas) de un cliente,
        // cada uno con su forma de pago. Si factura no es null, el recibo queda ligado
        // a esa venta (cobro de una venta al contado en Punto de Venta, en el mismo
        // momento de la venta); si es null, es un pago posterior contra el saldo
        // pendiente de una venta a crédito (registrado desde Estado de Cuenta).
        // Devuelve el número de recibo asignado.
        public static string RegistrarRecibo(int idCliente, DateTime fecha, string factura, List<LineaPago> lineas, string nota)
        {
            if (lineas == null || lineas.Count == 0)
            {
                throw new Exception("Agrega al menos una línea de pago.");
            }

            decimal total = 0;
            foreach (LineaPago linea in lineas)
            {
                if (linea.Monto <= 0)
                {
                    throw new Exception("Cada línea de pago debe tener un monto mayor a cero.");
                }
                total += linea.Monto;
            }

            string numeroRecibo = Busco.BuscaUltimoNumero("3");

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();

                using (SqlTransaction tx = cnx.BeginTransaction())
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand(
                            " INSERT INTO RECIBO (RECIBO, IDCLIENTE, FECHA, FACTURA, MONTO, NOTA, ACTIVO) " +
                            " VALUES (@recibo, @idCliente, @fecha, @factura, @monto, @nota, 1) ", cnx, tx);
                        cmd.Parameters.AddWithValue("@recibo", numeroRecibo);
                        cmd.Parameters.AddWithValue("@idCliente", idCliente);
                        cmd.Parameters.AddWithValue("@fecha", fecha.ToString("dd/MM/yyyy"));
                        cmd.Parameters.AddWithValue("@factura", (object)factura ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@monto", total);
                        cmd.Parameters.AddWithValue("@nota", (object)nota ?? DBNull.Value);
                        cmd.ExecuteNonQuery();

                        SqlCommand cmdSec = new SqlCommand("UPDATE SECUENCIA SET SECUENCIA = @numero WHERE id = 3", cnx, tx);
                        cmdSec.Parameters.AddWithValue("@numero", numeroRecibo);
                        cmdSec.ExecuteNonQuery();

                        foreach (LineaPago linea in lineas)
                        {
                            SqlCommand cmdDet = new SqlCommand(
                                "INSERT INTO DETALLERECIBO (RECIBO, IDTIPOPAGO, MONTO) VALUES (@recibo, @tipo, @monto)", cnx, tx);
                            cmdDet.Parameters.AddWithValue("@recibo", numeroRecibo);
                            cmdDet.Parameters.AddWithValue("@tipo", linea.IdTipoPago);
                            cmdDet.Parameters.AddWithValue("@monto", linea.Monto);
                            cmdDet.ExecuteNonQuery();
                        }

                        RegistrarMovimiento(cnx, tx, idCliente.ToString(), fecha, OrigenAbono, numeroRecibo, -total);

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }

            return numeroRecibo;
        }

        // Genera el PDF del recibo en Recibos\Recibo_<numero>.pdf y devuelve la ruta.
        public static string GenerarReciboPdf(string numeroRecibo, DateTime fecha, string clienteNombre, List<LineaPago> lineas, decimal total, string nota)
        {
            string ruta = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string carpeta = Path.Combine(ruta, "Recibos");
            Directory.CreateDirectory(carpeta);

            string archivo = Path.Combine(carpeta, "Recibo_" + numeroRecibo + ".pdf");

            Document doc = new Document();
            PdfWriter.GetInstance(doc, new FileStream(archivo, FileMode.Create));
            doc.Open();

            doc.Add(new Paragraph("RECIBO DE INGRESO"));
            doc.Add(new Paragraph("Numero: " + numeroRecibo));
            doc.Add(new Paragraph("Fecha: " + fecha.ToString("dd/MM/yyyy")));
            doc.Add(new Paragraph("Cliente: " + clienteNombre));
            doc.Add(new Paragraph(" "));

            foreach (LineaPago linea in lineas)
            {
                doc.Add(new Paragraph(linea.NombreTipoPago + ": " + linea.Monto.ToString("0.00")));
            }

            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph("Total: " + total.ToString("0.00")));
            if (!string.IsNullOrWhiteSpace(nota))
            {
                doc.Add(new Paragraph("Nota: " + nota));
            }

            doc.Close();

            return archivo;
        }

        private static void RegistrarMovimiento(SqlConnection cnx, SqlTransaction tx, string idClienteTexto, DateTime fecha, int origen, string documento, decimal monto)
        {
            int idCliente;
            if (!int.TryParse(idClienteTexto, out idCliente)) return;

            SqlCommand cmdSaldo = new SqlCommand(
                "SELECT ISNULL(SUM(MONTO), 0) FROM MUTOCTE WHERE IDCLIENTE = @id AND ACTIVO = 1", cnx, tx);
            cmdSaldo.Parameters.AddWithValue("@id", idCliente);
            decimal saldoAnterior = Convert.ToDecimal(cmdSaldo.ExecuteScalar());
            decimal saldoNuevo = saldoAnterior + monto;

            SqlCommand cmd = new SqlCommand(
                " INSERT INTO MUTOCTE (IDCLIENTE, FECHA, ORIGEN, DOCUMENTO, MONTO, BCPENDIENTE, ACTIVO) " +
                " VALUES (@idCliente, @fecha, @origen, @documento, @monto, @saldo, 1) ", cnx, tx);
            cmd.Parameters.AddWithValue("@idCliente", idCliente);
            cmd.Parameters.AddWithValue("@fecha", fecha.ToString("dd/MM/yyyy"));
            cmd.Parameters.AddWithValue("@origen", origen);
            cmd.Parameters.AddWithValue("@documento", (object)documento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@monto", monto);
            cmd.Parameters.AddWithValue("@saldo", saldoNuevo);
            cmd.ExecuteNonQuery();
        }

        // Saldo pendiente real del cliente, recalculado en vivo (no el bcPendiente
        // guardado en la última fila, que puede quedar desactualizado si un cargo
        // anterior se desactivó después).
        public static decimal ObtenerSaldoPendiente(int idCliente)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT ISNULL(SUM(MONTO), 0) FROM MUTOCTE WHERE IDCLIENTE = @id AND ACTIVO = 1", cnx);
                cmd.Parameters.AddWithValue("@id", idCliente);
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        public static List<MovimientoCuenta> ObtenerMovimientos(int idCliente)
        {
            List<MovimientoCuenta> lista = new List<MovimientoCuenta>();

            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT FECHA, ORIGEN, DOCUMENTO, MONTO, BCPENDIENTE FROM MUTOCTE " +
                    " WHERE IDCLIENTE = @id AND ACTIVO = 1 ORDER BY ID ", cnx);
                cmd.Parameters.AddWithValue("@id", idCliente);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        lista.Add(new MovimientoCuenta
                        {
                            Fecha = Convert.ToString(rdr["FECHA"]),
                            Documento = Convert.ToString(rdr["DOCUMENTO"]),
                            Monto = Convert.ToDecimal(rdr["MONTO"]),
                            SaldoDespues = rdr["BCPENDIENTE"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["BCPENDIENTE"]),
                            EsAbono = Convert.ToInt32(rdr["ORIGEN"]) == OrigenAbono
                        });
                    }
                }
            }

            return lista;
        }

        // Formas de pago usadas en un recibo (por ejemplo "Efectivo, Tarjeta" si se
        // repartió entre varias), para mostrar junto al movimiento en Estado de Cuenta.
        // Devuelve "" si el documento no corresponde a ningún recibo (por ejemplo, un
        // abono migrado de antes de este cambio, si lo hubiera).
        public static string ObtenerFormasPagoDeRecibo(string numeroRecibo)
        {
            using (SqlConnection cnx = new SqlConnection(cnn.db))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(
                    " SELECT TP.NOMBRE FROM DETALLERECIBO D " +
                    " INNER JOIN TIPOPAGO TP ON D.IDTIPOPAGO = TP.ID " +
                    " WHERE D.RECIBO = @recibo ORDER BY D.SECUENCIA ", cnx);
                cmd.Parameters.AddWithValue("@recibo", numeroRecibo);

                StringBuilder sb = new StringBuilder();
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        if (sb.Length > 0) sb.Append(", ");
                        sb.Append(Convert.ToString(rdr["NOMBRE"]));
                    }
                }

                return sb.ToString();
            }
        }
    }
}
