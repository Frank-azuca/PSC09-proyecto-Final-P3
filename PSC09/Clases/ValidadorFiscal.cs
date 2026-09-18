namespace PSC09
{
    // Validacion de Cedula/RNC dominicana para CLIENTES.idIdentificacion, que hoy es
    // un solo campo de texto libre sin un "tipo" separado (ver P2.2 en
    // cambios-y-mejoras-a-implementar.md para agregar esa columna mas adelante).
    //
    // A proposito NO bloquea el guardado si el digito verificador no cuadra -- solo
    // avisa (ver frmCliente.btnGuardar_Click): un typo es mas probable que un
    // documento real invalido, y una advertencia que se puede pasar por alto es mas
    // segura que rechazar de plano a un cliente real por un error del validador.
    //
    // Alcance de esta ronda: se implementa el digito verificador de la Cedula (11
    // digitos, algoritmo tipo Luhn con pesos 1,2 alternados), que es el que esta bien
    // documentado y es verificable de forma independiente. El RNC (9 digitos, para
    // negocios) solo se valida por longitud/formato -- no hay un algoritmo de digito
    // verificador para RNC igual de bien documentado publicamente, y mejor no validar
    // que validar mal el identificador fiscal real de un negocio.
    public static class ValidadorFiscal
    {
        public static bool PareceCedula(string identificacion)
        {
            return SoloDigitos(identificacion) && identificacion.Length == 11;
        }

        public static bool PareceRnc(string identificacion)
        {
            return SoloDigitos(identificacion) && identificacion.Length == 9;
        }

        // Si "cedula" no tiene pinta de Cedula (PareceCedula == false) no hay nada que
        // validar aqui y se devuelve true (no es su responsabilidad decir si un RNC o
        // un pasaporte son validos).
        public static bool DigitoVerificadorCedulaValido(string cedula)
        {
            if (!PareceCedula(cedula)) return true;

            int[] pesos = { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 };
            int suma = 0;
            for (int i = 0; i < 10; i++)
            {
                int producto = (cedula[i] - '0') * pesos[i];
                if (producto > 9) producto -= 9;
                suma += producto;
            }

            int verificador = (10 - (suma % 10)) % 10;
            int digitoReal = cedula[10] - '0';
            return verificador == digitoReal;
        }

        private static bool SoloDigitos(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return false;

            foreach (char c in texto)
            {
                if (!char.IsDigit(c)) return false;
            }

            return true;
        }
    }
}
