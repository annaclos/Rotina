using NFK001.Contracts.NFK001;
using NFK001.Models.NFK001;

namespace NFK001.Business.NFK001
{
    public class NFK001BLL : INFK001BLL
    {
        public readonly INFK001DAL _INFK001DAL;
        public NFK001BLL(INFK001DAL INFK001DAL)
        {
            _INFK001DAL = INFK001DAL;
        }

        public async Task<EnCodeProcess> Update(string data)
        {
            try
            {
                List<Response> resp = [];
                Util.Log("Inicia transação");
                _INFK001DAL.BeginTransaction();

                resp = await _INFK001DAL.Get();
               
                resp.ForEach(x => x.DATAPLANO = Convert.ToDateTime(data));

                Util.Log("Atualiza...");
                await _INFK001DAL.Update(resp);

                Util.Log("Commit transação");
                _INFK001DAL.Commit();

                return EnCodeProcess.Sucesso;
            }
            catch (Exception ex)
            {
                Util.Log("Rollback");
                _INFK001DAL.Rollback();
                Util.Log($"Erro: {ex.Message}", EnTipoLog.Erro);
                return EnCodeProcess.Erro;
            }
        }
    }
}