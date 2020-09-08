class DeploymentConfig
{
    public const string _version = "1.2.24.0";
    public const bool _isBeta =
#if BETA
 true;
#else
 false;
#endif
    public const string _appId =
#if BETA
 "{607be39e-852e-42ac-aaef-f0a4a5fc11bb}";
#else
 "{89599822-4e57-4b65-839f-818657c5d58a}";
#endif
}
