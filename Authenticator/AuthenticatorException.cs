using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace YetAnotherRelogger
{
	/// <summary>
	/// Base Authenticator exception class
	/// </summary>
	public class AuthenticatorException : ApplicationException
	{
		public AuthenticatorException()
			: base()
		{
		}

		public AuthenticatorException(string msg)
			: base(msg)
		{
		}

		public AuthenticatorException(string msg, Exception ex)
			: base(msg, ex)
		{
		}
	}

	/// <summary>
	/// Exception for reading invalid config data
	/// </summary>
	public class InvalidConfigDataException : AuthenticatorException
	{
		public InvalidConfigDataException() : base() {}
	}

	/// <summary>
	/// Exception for invalid user decryption
	/// </summary>
	public class InvalidUserDecryptionException : AuthenticatorException
	{
		public InvalidUserDecryptionException() : base() { }
	}

	/// <summary>
	/// Exception for invalid machine decryption
	/// </summary>
	public class InvalidMachineDecryptionException : AuthenticatorException
	{
		public InvalidMachineDecryptionException() : base() { }
	}

	/// <summary>
	/// Exception for error or unexpected return from server for enroll
	/// </summary>
	public class InvalidEnrollResponseException : AuthenticatorException
	{
		public InvalidEnrollResponseException(string msg) : base(msg) { }
	}

	/// <summary>
	/// Exception for error or unexpected return from server for sync
	/// </summary>
	public class InvalidSyncResponseException : AuthenticatorException
	{
		public InvalidSyncResponseException(string msg) : base(msg) { }
	}

	/// <summary>
	/// Config has been encryoted and we need a key
	/// </summary>
	public class EncrpytedSecretDataException : AuthenticatorException
	{
		public EncrpytedSecretDataException() : base() { }
	}

	/// <summary>
	/// Config decryption bad password
	/// </summary>
	public class BadPasswordException : AuthenticatorException
	{
		public BadPasswordException() : base() { }
	}

	public class InvalidRestoreResponseException : AuthenticatorException
	{
		public InvalidRestoreResponseException(string msg) : base(msg) { }
	}

	public class InvalidRestoreCodeException : InvalidRestoreResponseException
	{
		public InvalidRestoreCodeException(string msg) : base(msg) { }
	}

	/// <summary>
	/// Invalid encryption detected
	/// </summary>
	public class InvalidEncryptionException : AuthenticatorException
	{
		public InvalidEncryptionException(string plain, string password, string encrypted, string decrypted) : base()
		{
			Plain = plain;
			Password = password;
			Encrypted = encrypted;
			Decrypted = decrypted;
		}

		public string Plain { get; set; }
		public string Password { get; set; }
		public string Encrypted { get; set; }
		public string Decrypted { get; set; }
	}

	/// <summary>
	/// Error on setting secret data (invalid decoding) caused by corruption or wrong password
	/// </summary>
	public class InvalidSecretDataException : AuthenticatorException
	{
		public InvalidSecretDataException(Exception inner, string password, string encType, List<string> decrypted)
			: base("Error decoding Secret Data", inner)
		{
			Password = password;
			EncType = encType;
			Decrypted = decrypted;
		}

		public string Password { get; set; }
		public string EncType { get; set; }
		public List<string> Decrypted { get; set; }
	}
}
