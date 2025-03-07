using NFK001.Contracts.NFK001;
using NFK001.Infra;
using NFK001.Models.NFK001;

namespace NFK001.Business.NFK001
{
    public class NFK001DAL : DapperTransaction, INFK001DAL
    {
        private readonly DapperContext _context;

        public NFK001DAL(DapperContext dapperContext)
        {
            _context = dapperContext;
        }
        public async Task<List<Response>> Get()
        {
            IEnumerable<Response> retr = await _context
                .QueryAsync<Response>(NFK001DALSQL.Get(), Transaction);

            return retr.ToList();
        }

        public async Task Update(List<Response> request)
        {
            await _context.ExecuteBulkAsync(NFK001DALSQL.Update(), request, Transaction);
        }
    }
}
