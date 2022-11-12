namespace WebApi_Redes.Model
{
    public class VeiculoConfiguracao
    {
        
        public int Id { get; set; }
        public float DistanciaRodiviaPavimentada { get; set; }
        public float DistanciaRodoviaNaoPavimentada { get; set; }
        public TipoVeiculo TipoVeiculoUtilizado { get; set; }
        public int Carga { get; set; }
        public float CustoTotal { get; set; }

    }
}
