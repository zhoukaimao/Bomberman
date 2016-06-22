using System.Security.Cryptography;
using System.Text;

public class GlobalDataHelper
{
    private const string DATA_ENCRYPT_KEY = "a234857890654c3678d77234567890O2";
    private static RijndaelManaged _encryptAlgorithm = null;

    public static RijndaelManaged DataEncryptAlgorithm()
    {
        _encryptAlgorithm = new RijndaelManaged();
        _encryptAlgorithm.Key = Encoding.UTF8.GetBytes(DATA_ENCRYPT_KEY);
        _encryptAlgorithm.Mode = CipherMode.ECB;
        _encryptAlgorithm.Padding = PaddingMode.PKCS7;

        return _encryptAlgorithm;
    }
}
