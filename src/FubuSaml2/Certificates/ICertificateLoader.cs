﻿using System.Security.Cryptography.X509Certificates;

namespace FubuSaml2.Certificates
{
    public interface ICertificateLoader
    {
        X509Certificate2 Load(string thumbprint);
    }
}