// ************************************************************************************ //
// Source: http://www.codeproject.com/Articles/15280/Cryptography-for-the-NET-Framework //
// ************************************************************************************ //

public static class Crypto
{

private static void ValidateRSAKeys()
{
	if (!File.Exists(KEY_PRIVATE) || !File.Exists(KEY_PUBLIC)) {
		RSA rsa = RSA.Create;
		rsa.KeySize = KeySize.RSA;
		
		// Public key
		string publicKey = rsa.ToXmlString(false);
		using (StreamWriter publicFile = File.CreateText(KEY_PUBLIC))
		{
			publicFile.Write(publicKey);
		}
		
		// Private key	
		string privateKey = rsa.ToXmlString(true);
		using (StreamWriter privateFile = File.CreateText(KEY_PRIVATE))
		{
			privateFile.Write(privateKey);
		}
	}
}

private static byte[] RSAEncrypt(byte[] plainText)
{
	//Make sure that the public and private key exists
	ValidateRSAKeys();
	string publicKey = GetTextFromFile(KEY_PUBLIC);
	string privateKey = GetTextFromFile(KEY_PRIVATE);

	//The RSA algorithm works on individual blocks of unencoded bytes.
	// In this case, the maximum is 58 bytes. Therefore, we are required
	// to break up the text into blocks and encrypt each one individually. 
	//Each encrypted block will give us an output of 128 bytes.
	//If we do not break up the blocks in this manner, we will throw 
	//a "key not valid for use in specified state" exception

	//Get the size of the final block
	int blockCount = Math.Floor(plainText.Length / RSA_BLOCKSIZE);
	int lastBlockLength = plainText.Length % RSA_BLOCKSIZE;
	
	bool hasLastBlock = false;
	if (lastBlockLength > 0) {
		//We need to create a final block for the remaining characters
		blockCount += 1;
		hasLastBlock = true;
	}
	
	//Initialize the RSA Service Provider with the public key
	RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(KeySize.RSA);
	rsa.FromXmlString(publicKey);

	//Initialize the result buffer
	byte[] cipherText = new byte[];

	//Break the text into blocks and work on each block individually
	for (int blockIndex = 0; blockIndex < blockCount; blockIndex++) {
		
		//If this is the last block and we have a remainder, then set the length accordingly
		int thisBlockLength = (blockIndex = blockCount - 1 && hasLastBlock) ?
			lastBlockLength :
			RSA_BLOCKSIZE;	

		//Define the block that we will be working on	
		byte[] plainBlock = new byte[thisBlockLength];
		int startChar = blockIndex * RSA_BLOCKSIZE;
		Array.Copy(plainText, startChar, plainBlock, 0, thisBlockLength);

		//Encrypt the current block and append it to the result stream
		byte[] cipherBlock = rsa.Encrypt(plainBlock, false);
		
		int originalLength = cipherText.Length;
		Array.Resize(cipherText, originalLength + cipherBlock.Length);
		cipherBlock.CopyTo(cipherText, originalLength);
	}

	//Release any resources held by the RSA Service Provider
	rsa.Clear();

	return cipherText;
}

private static byte[] RSADecrypt(string cipherText)
{
	//Make sure that the public and private key exists
	ValidateRSAKeys();
	string publicKey = GetTextFromFile(KEY_PUBLIC);
	string privateKey = GetTextFromFile(KEY_PRIVATE);

	//When we encrypt a string using RSA, it works on individual blocks of up to
	//58 bytes. Each block generates an output of 128 encrypted bytes.
	//Therefore, to decrypt the message, we need to break the encrypted 
	//stream into individual chunks of 128 bytes and decrypt them individually
	//Determine how many bytes are in the encrypted stream. 
	//The input is in hex format, so we have to divide it by 2
	
	int maxBytes = cipherText.Length / 2;

	//Ensure that the length of the encrypted stream is divisible by 128
	if (!(maxBytes % RSA_DECRYPTBLOCKSIZE == 0)) {
		throw new System.Security.Cryptography.CryptographicException("Encrypted" + " text is an invalid length");
		return null;
	}

	//Calculate the number of blocks we will have to work on
	int blockCount = maxBytes / RSA_DECRYPTBLOCKSIZE;

	//Initialize the RSA Service Provider
	RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(KeySize.RSA);
	rsa.FromXmlString(privateKey);

	//Initialize the result buffer
	byte[] plainText = new byte[];
	
	//Iterate through each block and decrypt it
	for (int blockIndex = 0; blockIndex < blockCount; blockIndex++) {
		//Get the current block to work on
		dynamic currentBlockHex = cipherText.Substring(blockIndex * (RSA_DECRYPTBLOCKSIZE * 2), RSA_DECRYPTBLOCKSIZE * 2);
		byte[] cipherBlock = HexToBytes(currentBlockHex);

		//Decrypt the current block and append it to the result stream
		byte[] plainBlock = rsa.Decrypt(cipherBlock, false);
		
		int originalLength = plainText.Length;
		Array.Resize(plainText, originalLength + plainBlock.Length);
		plainBlock.CopyTo(plainText, originalLength);
	}

	//Release all resources held by the RSA service provider
	rsa.Clear();

	return plainText;
}

} // class Crypto

//
// Encrypt/Decrypt a string
//
Crypto.EncryptionAlgorithm = Crypto.Algorithm.Rijndael;
Crypto.Encoding = Crypto.EncodingType.BASE_64;
Crypto.Key = "This is @ key and IT 1s strong";

// Encrypt
if (Crypto.EncryptString("This is the string I want to encrypt")) {
	MessageBox.Show("The encrypted text is: " + Crypto.Content);
} else {
	MessageBox.Show(Crypto.CryptoException.Message);
}

// Decrypt
if (Crypto.DecryptString) {
	MessageBox.Show("The decrypted string is " + Crypto.Content);
} else {
	MessageBox.Show(Crypto.CryptoException.Message);
}

Crypto.Clear();

//
// Encrypt/Decrypt a file
//
Crypto.EncryptionAlgorithm = Crypto.Algorithm.RSA;
Crypto.Encoding = Crypto.EncodingType.HEX;
Crypto.Key = "This is @ key and IT 1s strong";

// Encrypt
if (Crypto.EncryptFile("c:\\MyTextFile.txt", "c:\\MyEncryptedFile.txt")) {
	MessageBox.Show("File Encrypted");
} else {
	MessageBox.Show(Crypto.CryptoException.Message);
}

// Decrypt
if (Crypto.DecryptFile("c:\\MyEncryptedFile.txt", "c:\\MyTextFile.txt")) {
	MessageBox.Show("File Decrypted");
} else {
	MessageBox.Show(Crypto.CryptoException.Message);
}

Crypto.Clear();


//
// Generate a hash
//
Crypto.EncryptionAlgorithm = Crypto.Algorithm.SHA512;
Crypto.Encoding = Crypto.EncodingType.HEX;
if (Crypto.GenerateHash("This is my password")) {
	MessageBox.Show("Hashed password is " + Crypto.Content);
} else {
	MessageBox.Show(Crypto.CryptoException.Message);
}
Crypto.Clear();
