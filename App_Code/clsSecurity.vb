Option Explicit On
Option Strict On

Imports BCrypt.Net.BCrypt
Imports System.IO
Imports System.Net
Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions

Public Class clsSecurity

    ' This class hold the functions that validate/secure data input

    Public Shared Function StripTags(str As String) As String
        Return Regex.Replace(str, "<.*?>", "")  ' return cleaned string
    End Function

    Public Shared Function StripTagsAndContent(str As String) As String
        Return Regex.Replace(str, "/<(\w+)[^>]*>.*<\/\1>/gi", "")  ' return cleaned string
    End Function

    Public Shared Function isValidPostCode(postCode As String) As Boolean
        Dim pattern As String = "^([A-PR-UWYZ]([0-9]{1,2}|([A-HK-Y][0-9]|[A-HK-Y][0-9]([0-9]|[ABEHMNPRV-Y]))|[0-9][A-HJKS-UW])\ [0-9][ABD-HJLNP-UW-Z]{2}|(GIR\ 0AA)|(SAN\ TA1)|(BFPO\ (C\/O\ )?[0-9]{1,4})|((ASCN|BBND|[BFS]IQQ|PCRN|STHL|TDCU|TKCA)\ 1ZZ))$"
        If postCode.Length < 9 Then Return Regex.IsMatch(postCode, pattern, RegexOptions.IgnoreCase)
        Return False
    End Function

    Public Shared Function isValidEmail(email As String) As Boolean
        Dim pattern As String = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$"
        Return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase)
    End Function

    Public Shared Function VerifyBCryptHash(password As String, hash As String) As Boolean
        Return BCrypt.Net.BCrypt.Verify(password, hash)
    End Function

    Public Shared Sub PreventBackButtonCaching(hr As HttpResponse)
        hr.Cache.SetNoStore()
        hr.Cache.AppendCacheExtension("no-cache")
        hr.Expires = 0
    End Sub

    Public Shared Function Sanitize(inputStr As String) As String
        Dim sb As New StringBuilder(inputStr)
        Dim outputStr As String

        ' find & replace the bad characters
        sb.Replace("--", "\-\-")
        sb.Replace("'", "\'")
        sb.Replace("=", "\=")
        sb.Replace(";", "\;\")
        sb.Replace("#", "\#\")
        outputStr = sb.ToString
        sb = Nothing

        ' return the cleaned string
        Return outputStr
    End Function

    Public Shared Function EncryptSHA256(str As String) As String
        ' this function returns a SHA256 hash based on the string passed 
        Dim uEncode As New UnicodeEncoding
        Dim byteStr() As Byte = uEncode.GetBytes(str)
        Dim sha As New System.Security.Cryptography.SHA256Managed
        Dim hash() As Byte = sha.ComputeHash(byteStr)

        Return Convert.ToBase64String(hash) ' returns 44 character hash 
    End Function

    Public Shared Function Encrypt(plainText As String) As String
        Dim passPhrase As String = "'@Ca;z[X,6G3#m3CgZg8"  ' 20 characters minimum
        Dim saltValue As String = "$CBA8f+6;Le^8[4W"  ' 16 characters
        Dim hashAlgorithm As String = "SHA1"
        Dim passwordIterations As Integer = 2
        Dim initVector As String = "vs2>a@vCE!hSV*?U"  ' 16 characters
        Dim keySize As Integer = 256
        Dim initVectorBytes As Byte() = Encoding.ASCII.GetBytes(initVector)
        Dim saltValueBytes As Byte() = Encoding.ASCII.GetBytes(saltValue)
        Dim plainTextBytes As Byte() = Encoding.UTF8.GetBytes(plainText)
        Dim password As New PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations)
        Dim keyBytes As Byte() = password.GetBytes(keySize \ 8)
        Dim symmetricKey As New RijndaelManaged()

        symmetricKey.Mode = CipherMode.CBC

        Dim encryptor As ICryptoTransform = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes)
        Dim memoryStream As New MemoryStream()
        Dim cryptoStream As New CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)

        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length)
        cryptoStream.FlushFinalBlock()

        Dim cipherTextBytes As Byte() = memoryStream.ToArray()
        memoryStream.Close()
        cryptoStream.Close()

        Dim cipherText As String = Convert.ToBase64String(cipherTextBytes)
        Return cipherText
    End Function

    Public Shared Function Decrypt(cipherText As String) As String
        Dim passPhrase As String = "'@Ca;z[X,6G3#m3CgZg8"  ' same as encrypt method value
        Dim saltValue As String = "$CBA8f+6;Le^8[4W"  ' same as encrypt method value
        Dim hashAlgorithm As String = "SHA1"
        Dim passwordIterations As Integer = 2
        Dim initVector As String = "vs2>a@vCE!hSV*?U"  ' same as encrypt method value
        Dim keySize As Integer = 256

        ' Convert strings defining encryption key characteristics into byte arrays. 
        ' Let us assume that strings only contain ASCII codes.
        ' If strings include Unicode characters, use Unicode, UTF7, or UTF8 encoding.
        Dim initVectorBytes As Byte() = Encoding.ASCII.GetBytes(initVector)
        Dim saltValueBytes As Byte() = Encoding.ASCII.GetBytes(saltValue)

        ' Convert our ciphertext into a byte array.
        Dim cipherTextBytes As Byte() = Convert.FromBase64String(cipherText)

        ' First, we must create a password, from which the key will be derived. 
        ' This password will be generated from the specified passphrase and salt value. 
        ' The password will be created using the specified hash algorithm. 
        ' Password creation can be done in several iterations.
        Dim password As New PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations)

        ' Use the password to generate pseudo-random bytes for the encryption
        ' key. Specify the size of the key in bytes (instead of bits).
        Dim keyBytes As Byte() = password.GetBytes(keySize \ 8)

        ' Create uninitialized Rijndael encryption object.
        Dim symmetricKey As New RijndaelManaged()

        ' It is reasonable to set encryption mode to Cipher Block Chaining (CBC). 
        ' Use default options for other symmetric key parameters.
        symmetricKey.Mode = CipherMode.CBC

        ' Generate decryptor from the existing key bytes and initialization vector.
        ' Key size will be defined based on the number of the key bytes.
        Dim decryptor As ICryptoTransform = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes)

        ' Define memory stream which will be used to hold encrypted data.
        Dim memoryStream As New MemoryStream(cipherTextBytes)

        ' Define cryptographic stream (always use Read mode for encryption).
        Dim cryptoStream As New CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)

        ' Since at this point we don't know what the size of decrypted data will be, allocate the buffer long enough to hold ciphertext;
        ' plaintext is never longer than ciphertext.
        Dim plainTextBytes As Byte() = New Byte(cipherTextBytes.Length - 1) {}

        ' Start decrypting.
        Dim decryptedByteCount As Integer = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length)

        ' Close both streams.
        memoryStream.Close()
        cryptoStream.Close()

        ' Convert decrypted data into a string. 
        ' Let us assume that the original plaintext string was UTF8-encoded.
        Dim plainText As String = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount)

        ' Return decrypted string.   
        Return plainText
    End Function

End Class
