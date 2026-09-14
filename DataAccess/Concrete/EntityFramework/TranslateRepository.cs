using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using Core.Entities.Dtos;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DataAccess.Concrete.EntityFramework
{
    public class TranslateRepository : EfEntityRepositoryBase<Translate, ProjectDbContext>, ITranslateRepository
    {
        public TranslateRepository(ProjectDbContext context)
            : base(context)
        {
        }

        public async Task<List<TranslateDto>> GetTranslateDto()
        {
            var list = await (from lng in Context.Languages
                join trs in Context.Translates on lng.Id equals trs.LangId
                select new TranslateDto()
                {
                    Id = trs.Id,
                    Code = trs.Code,
                    Language = lng.Code,
                    Value = trs.Value
                }).ToListAsync();

            return list;
        }

        public async Task<string> GetTranslatesByLang(string langCode)
        {
            var list = await (from trs in Context.Translates
                join lng in Context.Languages on trs.LangId equals lng.Id
                where lng.Code == langCode
                select new { Code = (string)trs.Code, Value = (string)trs.Value }).ToListAsync();

            var data = list.GroupBy(x => x.Code).ToDictionary(g => g.Key, g => g.First().Value);

            var str = JsonConvert.SerializeObject(data);
            return str;
        }

        public async Task<Dictionary<string, string>> GetTranslateWordList(string lang)
        {
            var list = await Context.Translates.Where(x => x.Code == lang).ToListAsync();

            return list.GroupBy(x => x.Code).ToDictionary(g => g.Key, g => g.First().Value);
        }
    }
}