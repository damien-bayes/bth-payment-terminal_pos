using CredentialManagement;
using System;

namespace IPTS
{
    interface ICredentialManager
    {
        CredentialData GetCredential();

        void SetCredential(string password, PersistanceType persistanceType);
    }
}
