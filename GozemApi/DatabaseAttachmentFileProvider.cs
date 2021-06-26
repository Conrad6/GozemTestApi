using System;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

using Raven.Client.Documents;

namespace GozemApi {
    public class DatabaseAttachmentFileProvider : IFileProvider
    {
        private readonly IDocumentStore documentStore;

        public DatabaseAttachmentFileProvider(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            throw new NotImplementedException();
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            return new DatabaseAttachmentFileInfo(documentStore, subpath);
        }

        public IChangeToken Watch(string filter)
        {
            throw new NotImplementedException();
        }
    }
}