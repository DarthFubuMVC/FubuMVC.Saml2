﻿using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace FubuSaml2.Encryption
{
    public interface IAssertionXmlEncryptor
    {
        void Encrypt(XmlDocument document, X509Certificate2 certificate);
    }
}