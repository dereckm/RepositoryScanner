using System;
using System.IO;
using System.Text;
using System.Xml;

namespace RepositoryReaders.Xml
{
    public class DefaultXmlReader : IXmlReader
    {
        private XmlReader _concreteReader;

        public void Create(string path)
        {
            _concreteReader = XmlReader.Create(path);
        }

        public bool ReadToFollowing(string name)
        {
            try
            {
                return _concreteReader.ReadToFollowing(name);
            }
            catch (XmlException xmlException)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var enc1252 = Encoding.GetEncoding(1252);
                var reader = new StreamReader(Uri.UnescapeDataString(new Uri(_concreteReader.BaseURI).AbsolutePath), enc1252, true);
                _concreteReader = XmlReader.Create(reader);
            }

            return false;
        }

        public string GetAttribute(string name)
        {
            return _concreteReader.GetAttribute(name);
        }
    }
}
