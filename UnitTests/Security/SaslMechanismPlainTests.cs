﻿//
// SaslMechanismPlainTests.cs
//
// Author: Jeffrey Stedfast <jestedfa@microsoft.com>
//
// Copyright (c) 2013-2023 .NET Foundation and Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System.Net;
using System.Text;

using MailKit.Security;

namespace UnitTests.Security {
	[TestFixture]
	public class SaslMechanismPlainTests
	{
		[Test]
		public void TestArgumentExceptions ()
		{
			var credentials = new NetworkCredential ("username", "password");
			SaslMechanism sasl;

			sasl = new SaslMechanismPlain (credentials);
			Assert.DoesNotThrow (() => sasl.Challenge (null));

			Assert.Throws<ArgumentNullException> (() => new SaslMechanismPlain (null, credentials));
			Assert.Throws<ArgumentNullException> (() => new SaslMechanismPlain (Encoding.UTF8, null));
			Assert.Throws<ArgumentNullException> (() => new SaslMechanismPlain (null));
			Assert.Throws<ArgumentNullException> (() => new SaslMechanismPlain (null, "username", "password"));
			Assert.Throws<ArgumentNullException> (() => new SaslMechanismPlain (Encoding.UTF8, null, "password"));
			Assert.Throws<ArgumentNullException> (() => new SaslMechanismPlain (Encoding.UTF8, "username", null));
			Assert.Throws<ArgumentNullException> (() => new SaslMechanismPlain (null, "password"));
			Assert.Throws<ArgumentNullException> (() => new SaslMechanismPlain ("username", null));
		}

		static void AssertPlain (SaslMechanismPlain sasl, string prefix)
		{
			const string expected = "AHVzZXJuYW1lAHBhc3N3b3Jk";
			string challenge;

			Assert.IsFalse (sasl.SupportsChannelBinding, "{0}: SupportsChannelBinding", prefix);
			Assert.IsTrue (sasl.SupportsInitialResponse, "{0}: SupportsInitialResponse", prefix);

			challenge = sasl.Challenge (string.Empty);

			Assert.AreEqual (expected, challenge, "{0}: challenge response does not match the expected string.", prefix);
			Assert.IsTrue (sasl.IsAuthenticated, "{0}: should be authenticated.", prefix);
			Assert.IsFalse (sasl.NegotiatedChannelBinding, "{0}: NegotiatedChannelBinding", prefix);
			Assert.IsFalse (sasl.NegotiatedSecurityLayer, "{0}: NegotiatedSecurityLayer", prefix);

			Assert.AreEqual (string.Empty, sasl.Challenge (string.Empty), "{0}: challenge while authenticated.", prefix);
		}

		[Test]
		public void TestPlainAuth ()
		{
			var credentials = new NetworkCredential ("username", "password");
			var sasl = new SaslMechanismPlain (credentials);

			AssertPlain (sasl, "NetworkCredential");

			sasl = new SaslMechanismPlain ("username", "password");

			AssertPlain (sasl, "user/pass");
		}

		[Test]
		public void TestPlainWithAuthorizationId ()
		{
			const string expected = "YXV0aHppZAB1c2VybmFtZQBwYXNzd29yZA==";
			var sasl = new SaslMechanismPlain ("username", "password") { AuthorizationId = "authzid" };
			string challenge;

			Assert.IsFalse (sasl.SupportsChannelBinding, "SupportsChannelBinding");
			Assert.IsTrue (sasl.SupportsInitialResponse, "SupportsInitialResponse");

			challenge = sasl.Challenge (string.Empty);

			Assert.AreEqual (expected, challenge, "challenge response does not match the expected string.");
			Assert.IsTrue (sasl.IsAuthenticated, "should be authenticated.");
			Assert.IsFalse (sasl.NegotiatedChannelBinding, "NegotiatedChannelBinding");
			Assert.IsFalse (sasl.NegotiatedSecurityLayer, "NegotiatedSecurityLayer");

			Assert.AreEqual (string.Empty, sasl.Challenge (string.Empty), "challenge while authenticated.");
		}
	}
}
