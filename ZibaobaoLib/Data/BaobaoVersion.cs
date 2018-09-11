using System;
using Newtonsoft.Json;

namespace ZibaobaoLib.Data
{
    public class BaobaoVersion : IComparable, IComparable<BaobaoVersion>
    {
        public int VersionMajor { get; set; }
        public int VersionMiddle { get; set; }
        public int VersionMinor { get; set; }
        public int VersionBuild { get; set; }

        [JsonIgnore]
        public string VersionString => $"{VersionMajor}.{VersionMiddle}.{VersionMinor}.{VersionBuild}";

        public static BaobaoVersion FromString(string versionString)
        {
            var ver = new BaobaoVersion();
            var versions = versionString.Split('.');
            int verionNumber = 0;
            if (versions.Length >= 4)
            {
                if (int.TryParse(versions[3], out verionNumber))
                {
                    ver.VersionBuild = verionNumber;
                }
            }

            if (versions.Length >= 3)
            {
                if (int.TryParse(versions[2], out verionNumber))
                {
                    ver.VersionMinor = verionNumber;
                }
            }

            if (versions.Length >= 2)
            {
                if (int.TryParse(versions[1], out verionNumber))
                {
                    ver.VersionMiddle = verionNumber;
                }
            }

            if (versions.Length >= 1)
            {
                if (int.TryParse(versions[0], out verionNumber))
                {
                    ver.VersionMajor = verionNumber;
                }
            }
            return ver;
        }
        #region Helpers
        public void UpdateVersion(BaobaoVersion newVersion)
        {
            VersionMajor = newVersion.VersionMajor;
            VersionMiddle = newVersion.VersionMiddle;
            VersionMinor = newVersion.VersionMinor;
            VersionBuild = newVersion.VersionBuild;
        }
        public static int Comparison(BaobaoVersion version1, BaobaoVersion version2)
        {
            if (ReferenceEquals(version1, version2))
            {
                return 0;
            }
            if (ReferenceEquals(version1, null))
            {
                return -1;
            }
            if (ReferenceEquals(version2, null))
            {
                return 1;
            }
            if (version1.VersionMajor > version2.VersionMajor)
            {
                return 1;
            }

            if (version1.VersionMajor < version2.VersionMajor)
            {
                return -1;
            }

            if (version1.VersionMiddle > version2.VersionMiddle)
            {
                return 1;
            }

            if (version1.VersionMiddle < version2.VersionMiddle)
            {
                return -1;
            }

            if (version1.VersionMinor > version2.VersionMinor)
            {
                return 1;
            }

            if (version1.VersionMinor < version2.VersionMinor)
            {
                return -1;
            }

            if (version1.VersionBuild > version2.VersionBuild)
            {
                return 1;
            }

            if (version1.VersionBuild < version2.VersionBuild)
            {
                return -1;
            }
            return 0;
        }
        public int CompareTo(object obj)
        {
            return CompareTo(obj as BaobaoVersion);
        }
        public int CompareTo(BaobaoVersion other)
        {
            return Comparison(this, other);
        }

        public static bool operator <(BaobaoVersion version1, BaobaoVersion version2)
        {
            return Comparison(version1, version2) < 0;
        }

        public static bool operator >(BaobaoVersion version1, BaobaoVersion version2)
        {
            return Comparison(version1, version2) > 0;
        }

        public static bool operator ==(BaobaoVersion version1, BaobaoVersion version2)
        {
            return Comparison(version1, version2) == 0;
        }

        public static bool operator !=(BaobaoVersion version1, BaobaoVersion version2)
        {
            return Comparison(version1, version2) != 0;
        }

        public override bool Equals(object obj)
        {
            return CompareTo(obj as BaobaoVersion) == 0;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{VersionString}";
        }
        #endregion
    }
}