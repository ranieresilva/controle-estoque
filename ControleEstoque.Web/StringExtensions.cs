namespace ControleEstoque.Web
{
    public static class StringExtensions
    {
        public static int ToInt32(this string valor)
        {
            int.TryParse(valor, out int ret);
            return ret;
        }

        public static decimal ToDecimal(this string valor)
        {
            decimal.TryParse(valor, out decimal ret);
            return ret;
        }
    }
}