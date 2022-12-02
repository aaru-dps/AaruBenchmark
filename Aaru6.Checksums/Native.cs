using System.Runtime.InteropServices;

namespace Aaru6.Checksums;

public static class Native
{
    static bool _checked;
    static bool _supported;

    public static bool ForceManaged { get; set; }

    public static bool IsSupported
    {
        get
        {
            if(ForceManaged)
                return false;

            if(_checked)
                return _supported;

            ulong version;
            _checked = true;

            try
            {
                version = get_acn_version();
            }
            catch
            {
                _supported = false;

                return false;
            }

            // TODO: Check version compatibility
            _supported = version >= 0x06000000;

            return _supported;
        }
    }

    [DllImport("libAaru.Checksums.Native", SetLastError = true)]
    static extern ulong get_acn_version();
}