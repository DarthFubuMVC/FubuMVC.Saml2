﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace FubuSaml2.Certificates
{
    public class CertificateService : ICertificateService
    {
        private readonly ICertificateLoader _loader;
        private readonly ISamlCertificateRepository _repository;

        public CertificateService(ICertificateLoader loader, ISamlCertificateRepository repository)
        {
            _loader = loader;
            _repository = repository;
        }

        public CertificateResult Validate(SamlResponse response)
        {
            if (!MatchesIssuer(response)) return CertificateResult.CannotMatchIssuer;

            return response.Certificates.Any(x => x.IsVerified)
                       ? CertificateResult.Validated
                       : CertificateResult.NoValidCertificates;
        }

        public X509Certificate2 LoadCertificate(Uri issuer)
        {
            var samlCertificate = _repository.Find(issuer);

            if (samlCertificate == null) return null;

            return _loader.Load(samlCertificate.Thumbprint);
        }

        // virtual for testing
        public virtual bool MatchesIssuer(SamlResponse response)
        {
            var samlCertificate = _repository.Find(response.Issuer);
            if (samlCertificate == null) return false;

            return response.Certificates.Any(x => samlCertificate.Matches(x));
        }

    }
}