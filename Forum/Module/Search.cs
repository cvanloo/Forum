using System.Collections.Generic;
using System.Linq;
using Forum.Entity;
using Microsoft.EntityFrameworkCore;

namespace Forum.Module
{
    public class Search
    {
        private readonly Model.Database _dbContext;

        public Search(Model.Database dbContext)
        {
            _dbContext = dbContext;
        }
       
        public List<Thread> SearchThreadsByTitle(string title)
        {
            return _dbContext.Threads
                .Include(t => t.Creator)
                .Include(t => t.Forum)
                .Include(t => t.Tags)
                .Where(t => t.Title.Contains(title)).ToList();
        }

        public List<Thread> SearchThreadsByTag(IEnumerable<Tag> tags)
        {
            var foundThreads = new List<Thread>();
            
            foreach (var tag in tags)
            {
                foundThreads.AddRange
                (
                    _dbContext.Threads
                        .Include(t => t.Creator)
                        .Include(t => t.Forum)
                        .Include(t => t.Tags)       
                        .Where(t => t.Tags.Contains(tag)).ToList()
                );
            }
            
            return foundThreads;
        }

        public List<Thread> SearchThreadsByUser(string name)
        {
            return _dbContext.Threads
                .Include(t => t.Creator)
                .Include(t => t.Forum)
                .Include(t => t.Tags)       
                .Where(t => t.Creator.AccountName.Contains(name)).ToList();
        }
    }
}