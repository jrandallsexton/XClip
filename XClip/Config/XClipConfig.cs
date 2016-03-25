using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XClip.Config
{
    public class XClipConfig : ConfigurationSection
    {
        public static XClipConfig GetSection()
        {
            var section = ConfigurationManager.GetSection("XClipConfig") as XClipConfig;
            if (section == null)
                throw new ConfigurationErrorsException("The <XClipConfig> configuration section is not defined.");
            return section;
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("description", IsRequired = true, IsKey = false)]
        public string Description
        {
            get { return (string)this["description"]; }
        }

        [ConfigurationProperty("dirIn", IsRequired = true, IsKey = true)]
        public string DirectoryIn
        {
            get { return (string)this["dirIn"]; }
        }

        [ConfigurationProperty("dirOut", IsRequired = true, IsKey = true)]
        public string DirectoryOut
        {
            get { return (string)this["dirOut"]; }
        }
    }
}
