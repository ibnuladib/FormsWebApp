using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using FormsWebApplication.Models;
using Directory = System.IO.Directory;
using Microsoft.EntityFrameworkCore;

namespace FormsWebApplication.Services
{
    public class LuceneSearchService
    {
        private readonly string _indexPath = Path.Combine(Directory.GetCurrentDirectory(), "LuceneIndex");
        private readonly LuceneVersion _luceneVersion = LuceneVersion.LUCENE_48;

        public LuceneSearchService()
        {
            EnsureIndexDirectoryExists();
        }

        private void EnsureIndexDirectoryExists()
        {
            if (!Directory.Exists(_indexPath))
                Directory.CreateDirectory(_indexPath);
        }

        public List<Template> SearchTemplates(string queryText)
        {
            using var dir = FSDirectory.Open(new DirectoryInfo(_indexPath));
            using var reader = DirectoryReader.Open(dir);
            var searcher = new IndexSearcher(reader);
            using var analyzer = new StandardAnalyzer(_luceneVersion);
            var parser = new MultiFieldQueryParser(_luceneVersion, new[] { "Title", "Description" }, analyzer);
            var query = parser.Parse(queryText);

            var hits = searcher.Search(query, 10).ScoreDocs;
            var results = new List<Template>();

            foreach (var hit in hits)
            {
                var doc = searcher.Doc(hit.Doc);
                var Email = doc.Get("Email") ?? "Unknown";
                results.Add(new Template
                {
                    Id = int.Parse(doc.Get("Id")),
                    AuthorId = doc.Get("AuthorId") ?? "0",
                    Title = doc.Get("Title"),
                    Description = doc.Get("Description"),
                    Author = new ApplicationUser { Email = Email }
                });

            }

            return results;
        }



        public void Reindex(List<Template> templates)
        {
            using var dir = FSDirectory.Open(new DirectoryInfo(_indexPath));
            using var analyzer = new StandardAnalyzer(_luceneVersion);
            var config = new IndexWriterConfig(_luceneVersion, analyzer) { OpenMode = OpenMode.CREATE };
            using var writer = new IndexWriter(dir, config);

            foreach (var template in templates)
            {

                var Email = template.Author?.Email ?? "Unknown"; 

                var doc = new Document
                    {
                        new StringField("Id", template.Id.ToString(), Field.Store.YES),
                        new StringField("AuthorId", template.AuthorId.ToString(), Field.Store.YES),
                        new StringField("Email", Email, Field.Store.YES), 
                        new TextField("Title", template.Title, Field.Store.YES),
                        new TextField("Description", template.Description ?? "", Field.Store.YES)
                    };

                writer.AddDocument(doc);
            }

            writer.Commit();
        }
    }
}
