﻿using FubuSaml2.Certificates;

namespace FubuSaml2.Validation
{
    public class CertificateValidation : ISamlValidationRule
    {
        private readonly ICertificateService _service;

        public CertificateValidation(ICertificateService service)
        {
            _service = service;
        }

        public void Validate(SamlResponse response)
        {
            var key = _service.Validate(response);
            if (key != SamlValidationKeys.ValidCertificate)
            {
                response.LogError(key);
            }
        }
    }
}