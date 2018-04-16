using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace LINQPadHelpers.Certificates
{
    public static class CertificateUtils
    {
        public static IEnumerable<FoundCertificate> RetrieveCertificatesBy(X509FindType findType, string findValue, bool validOnly = false)
        {
            var storeNames = Enum.GetValues(typeof(StoreName)).Cast<StoreName>();
            var storeLocations = Enum.GetValues(typeof(StoreLocation)).Cast<StoreLocation>();
            var stores = storeLocations.SelectMany(x => storeNames.Select(y => new { Location = x, Name = y }));

            return stores.SelectMany(store => TryFindCertificates(new X509Store(store.Name, store.Location), findType, findValue, validOnly)
                .Select(x => new FoundCertificate { StoreLocation = store.Location, StoreName = store.Name, Certificate = x }));
        }

        public static IEnumerable<X509Certificate2> TryFindCertificates(X509Store store, X509FindType findType, string findValue, bool validOnly)
        {
            try
            {
                store.Open(OpenFlags.ReadOnly);

                return store.Certificates
                    .Find(findType, findValue, validOnly)
                    .OfType<X509Certificate2>();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error reading certificates from {0} / {1}: {2}", store.Location, store.Name, ex.Message);
            }
            finally
            {
                store.Close();
            }

            return null;
        }
    }
}