using ConsoleAppLibrary;
using Microsoft.AspNetCore.Mvc;

namespace WordCounterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordCounterController : ControllerBase
    {
        [HttpPost]
        public Dictionary<string, int> Post([FromBody] string content)
        {
            var wordCount = new WordCounterLib();
            var dictionaryParallel = wordCount.CountingWordsParallel(content);

            return dictionaryParallel;
        }
    }
}
