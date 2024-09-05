using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using SearchRepo.API.Domain.Models;
using Microsoft.Extensions.Configuration;

namespace SearchRepo.API.Filters
{
    public class AuthorizeFilter : IAsyncAuthorizationFilter
    {
        private IConfiguration _configuration;

        public AuthorizeFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            string jwt = context.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(jwt))
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            await IsValidJwt(context, jwt);
        }

        private async Task IsValidJwt(AuthorizationFilterContext context, string token)
        {
            string[] parts = token.Split('.');
            string header = parts[0];
            string payload = parts[1];
            string headerJson = Encoding.UTF8.GetString(Base64UrlDecoder(header));
            string payloadJson = Encoding.UTF8.GetString(Base64UrlDecoder(payload));
            JObject payloadData = JObject.Parse(payloadJson);
            if (!IsExpiredJwt(payloadData))
            { 
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            string certPublicKey = _configuration.GetValue<string>("AppSettings:certPublicKey");
            var keyBytes = Convert.FromBase64String(certPublicKey);

            //This code will verify the signature of the jwt, I need real public key for doing this.
            // I will assume it's passed the verify signature process...

            /*
            AsymmetricKeyParameter asymmetricKeyParameter = PublicKeyFactory.CreateKey(keyBytes);
            RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;
            RSAParameters rsaParameters = new RSAParameters();
            rsaParameters.Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned();
            rsaParameters.Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaParameters);

            SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(parts[0] + '.' + parts[1]));

            RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            rsaDeformatter.SetHashAlgorithm("SHA256");
            if (!rsaDeformatter.VerifySignature(hash, FromBase64Url(parts[2])))
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
            */
            var tokenData = await GetDataFromToken(payloadData);
            if (tokenData == null)
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            context.HttpContext.Items["TokenData"] = tokenData;
        }

        private async Task<TokenData> GetDataFromToken(JObject payloadData)
        {
            var mobile = payloadData["mobile"].ToString();
            var partyId = payloadData["partyId"].ToString();

            TokenData tokenData = new TokenData();
            tokenData.mobile = mobile;
            tokenData.partyId = partyId;
            return await Task.FromResult<TokenData>(tokenData);
        }

        private bool IsExpiredJwt(JObject payloadData)
        {
            var expiredDateTime = payloadData["exp"].ToString();
            if (string.IsNullOrEmpty(expiredDateTime))
            {
                return false;
            }

            DateTime dateTime = UnixTimeStampToDateTime(double.Parse(expiredDateTime));
            if (dateTime < DateTime.Now)
            {
                return false;
            }
            return true; 
        }

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            //Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        private static byte[] Base64UrlDecoder(string input)
        {
            var output = input;
            // 62nd char of encoding
            output = output.Replace('-', '+');
            //63rd char of encoding
            output = output.Replace('_', '/');
            switch(output.Length % 4)
            {
                case 0: // No pad chars in this case
                    break;
                case 1: // Three pad chars
                    output += "===";
                    break;
                case 2: // Two pad chars
                    output += "==";
                    break;
                case 3: // One pad chars
                    output += "=";
                    break;
                default: throw new Exception("Illegal base64url string");
            }

            //Standard base64 decoder
            var converted = Convert.FromBase64String(output);
            return converted;
        }

        private static byte[] FromBase64Url(string base64Url)
        {
            string padded = base64Url.Length % 4 == 0 ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
            string base64 = padded.Replace("_", "/")
                .Replace("-", "+");
            return Convert.FromBase64String(base64);
        }
    }
}
