namespace NFK001
{
    public enum EnTipoLog
    {
        /// <summary>
        /// Log padrão com horas.
        /// </summary>
        Padrao = 0,
        /// <summary>
        /// Log para separação de sessões.
        /// </summary>
        Divisor = 1,
        /// <summary>
        /// Log de início.
        /// </summary>
        Inicio = 2,
        /// <summary>
        /// Log de fim.
        /// </summary>
        Fim = 3,
        /// <summary>
        /// Log só com o informado pelo dev.
        /// </summary>
        Limpo = 4,
        /// <summary>
        /// Log com exception
        /// </summary>
        Erro = 5
    }

    public enum EnCodeProcess
    {
        /// <summary>
        /// Finalizado com sucesso
        /// </summary>
        Sucesso = 0,
        /// <summary>
        /// Finalizado com erros
        /// </summary>
        Erro = 12
    }
}
