// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace McMaster.AspNetCore.LetsEncrypt.Internal
{
    /// <summary>
    /// Loads certificates for all configured hostnames
    /// </summary>
    internal class AcmeCertificateLoader : IHostedService
    {
        private readonly CertificateSelector _selector;
        private readonly IHttpChallengeResponseStore _challengeStore;
        private readonly ICertificateStore _certificateStore;
        private readonly IOptions<LetsEncryptOptions> _options;
        private readonly ILogger<AcmeCertificateLoader> _logger;

        private volatile bool _hasRegistered;

        public AcmeCertificateLoader(
            CertificateSelector selector,
            IHttpChallengeResponseStore challengeStore,
            ICertificateStore certificateStore,
            IOptions<LetsEncryptOptions> options,
            ILogger<AcmeCertificateLoader> logger)
        {
            _selector = selector;
            _challengeStore = challengeStore;
            _certificateStore = certificateStore;
            _options = options;
            _logger = logger;
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_options.Value.HostNames.Length >= 0)
            {
                // load certificates in the background
                Task.Run(async () => await LoadCerts(cancellationToken));
            }

            return Task.CompletedTask;
        }

        private async Task LoadCerts(CancellationToken cancellationToken)
        {
            var errors = new List<Exception>();

            using (var factory = new CertificateFactory(_options, _challengeStore, _logger))
            {
                foreach (var hostName in _options.Value.HostNames)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        var cert = await GetOrCreateCertificate(factory, hostName, cancellationToken);
                        _selector.Use(hostName, cert);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex);
                    }
                }
            }

            if (errors.Count > 0)
            {
                throw new AggregateException(errors);
            }
        }

        private async Task<X509Certificate2> GetOrCreateCertificate(CertificateFactory factory, string hostName, CancellationToken cancellationToken)
        {
            var cert = _certificateStore.GetCertificate(hostName);
            if (cert != null)
            {
                _logger.LogDebug("Certificate for {hostname} already found.", hostName);
                return cert;
            }

            if (!_hasRegistered)
            {
                _hasRegistered = true;
                await factory.RegisterUserAsync(cancellationToken);
            }

            try
            {
                _logger.LogInformation("Creating certificate for {hostname} using ACME server {acmeServer}", hostName, _options.Value.AcmeServer);
                cert = await factory.CreateCertificateAsync(hostName, cancellationToken);
                _logger.LogInformation("Created certificate {subjectName} ({thumbprint})", cert.Subject, cert.Thumbprint);
                _certificateStore.Save(hostName, cert);
                return cert;
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, $"Failed to automatically create a certificate for {hostName};{ex.Message}", hostName);
                throw;
            }
        }
    }
}
