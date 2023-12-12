using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;
using YINSA_INTRANET.Models;

namespace YINSA_INTRANET.Servicios
{
    public class HashService
    {
        private static string Hash(string textoPlano)
        {
            var sal = "LKJUIDNDASFKJ.1.2?3KJHJGUY34A?";
            byte[] salt = Convert.FromBase64String(sal);
            var llaveDerivada = KeyDerivation.Pbkdf2(password: textoPlano,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 32);

            var hash = Convert.ToBase64String(llaveDerivada);

            return  hash;
        }
		public static string ConvertirSha256(string texto)
		{

			StringBuilder Sb = new();
			using (SHA256 hash = SHA256.Create())
			{
				Encoding enc = Encoding.UTF8;
				byte[] result = hash.ComputeHash(enc.GetBytes(texto));

				foreach (byte b in result)
					Sb.Append(b.ToString("x2"));
			}

			return Sb.ToString();
		}	
	}
}
