using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Extensions.FileProviders;

using Raven.Client.Documents;

namespace GozemApi {
    public class DatabaseAttachmentFileInfo : IFileInfo
    {
        private byte[] buffer;

        public DatabaseAttachmentFileInfo(IDocumentStore documentStore, string subpath)
        {
            var parts = subpath.Split('/')
                .ToArray();

            var documentId = parts.FirstOrDefault(x => x.Contains(documentStore.Conventions.IdentityPartsSeparator)) ?? string.Empty;
            var fileName = parts.FirstOrDefault(x => Regex.IsMatch(x, @"^.{36}\.(.){1,}$")) ?? string.Empty;

            using var session = documentStore.OpenSession();

            var attachment = session.Advanced.Attachments.Get(documentId, fileName);

            using var stream = new MemoryStream();
            attachment.Stream.CopyTo(stream);

            buffer = stream.ToArray();

            PhysicalPath = null;
            Name = fileName;
            Exists = true;
            IsDirectory = false;
            LastModified = new DateTimeOffset(DateTime.Now);
            Length = stream.Length;
        }

        public bool Exists { get; }
        public bool IsDirectory { get; }
        public DateTimeOffset LastModified { get; }
        public long Length { get; }
        public string Name { get; }
        public string PhysicalPath { get; }

        public Stream CreateReadStream()
        {
            return new MemoryStream(buffer);
        }
    }
}