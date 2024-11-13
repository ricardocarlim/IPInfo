namespace app.Helper
{
    public class Image
    {
        public static string ConverterParaBase64(IFormFile arquivo)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                arquivo.CopyTo(ms);
                byte[] bytes = ms.ToArray();
                string base64String = Convert.ToBase64String(bytes);
                return base64String;
            }
        }
    }
}
