using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebApi_Redes.Model;

namespace WebApi_Redes.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : ControllerBase
    {

        private readonly List<TipoVeiculo> TipoVeiculoList = new()
        {
            new TipoVeiculo {NomeTipoVeiculo = "Veículo urbano de carga (VUC)", FatorMultiplicacao = (float)1.00},
            new TipoVeiculo {NomeTipoVeiculo = "Caminhão 3/4", FatorMultiplicacao = (float)1.05},
            new TipoVeiculo {NomeTipoVeiculo = "Caminhão toco", FatorMultiplicacao = (float)1.08},
            new TipoVeiculo {NomeTipoVeiculo = "Carreta simples", FatorMultiplicacao = (float)1.13},
            new TipoVeiculo {NomeTipoVeiculo = "Carreta eixo estendido", FatorMultiplicacao = (float)1.19}

        };
        private readonly List<VeiculoConfiguracao> VeiculoConfiguracaoList = new()
        {
            new VeiculoConfiguracao {Id = 0, DistanciaRodiviaPavimentada = 90, DistanciaRodoviaNaoPavimentada = 0, TipoVeiculoUtilizado =  new TipoVeiculo{NomeTipoVeiculo = "Carreta simples", FatorMultiplicacao = (float)1.13 }, Carga = 80 },
            new VeiculoConfiguracao {Id = 1, DistanciaRodiviaPavimentada = 0, DistanciaRodoviaNaoPavimentada = 85, TipoVeiculoUtilizado =  new TipoVeiculo{NomeTipoVeiculo = "Veículo urbano de carga (VUC)", FatorMultiplicacao = (float)1.00}, Carga = 1 },
            new VeiculoConfiguracao {Id = 2, DistanciaRodiviaPavimentada = 20, DistanciaRodoviaNaoPavimentada = 80, TipoVeiculoUtilizado =  new TipoVeiculo{NomeTipoVeiculo = "Carreta eixo estendido", FatorMultiplicacao = (float)1.19}, Carga = 12 },
            new VeiculoConfiguracao {Id = 3, DistanciaRodiviaPavimentada = 70, DistanciaRodoviaNaoPavimentada = 20, TipoVeiculoUtilizado =  new TipoVeiculo{NomeTipoVeiculo = "Caminhão toco", FatorMultiplicacao = (float)1.08}, Carga = 5 }

        };


        [HttpPost]
        public ActionResult<List<TipoVeiculo>> PostAtualizaFatorMultiplicacao([FromBody] TipoVeiculo _tipoVeiculo)
        {
            TipoVeiculo tipoVeiculo = TipoVeiculoList.FirstOrDefault(x => x.NomeTipoVeiculo == _tipoVeiculo.NomeTipoVeiculo);

            if (tipoVeiculo is null)
                return BadRequest("Veículo não encontrado.");

            TipoVeiculoList.Remove(tipoVeiculo);
            tipoVeiculo.FatorMultiplicacao = _tipoVeiculo.FatorMultiplicacao;
            TipoVeiculoList.Add(tipoVeiculo);


            return Ok(TipoVeiculoList);
        }

        [HttpGet]
        public ActionResult<List<TipoVeiculo>> GetFatorMultiplicacao()
        {
            return Ok(TipoVeiculoList);
        }



        [HttpPost]
        public ActionResult<VeiculoConfiguracao> PostCriaRegistroVeiculoConfiguracao([FromBody] VeiculoConfiguracao _VeiculoConfiguracao)
        {
            _VeiculoConfiguracao.CustoTotal = AplicaCustoPavimentacao(_VeiculoConfiguracao);
            AplicaCustoTipoVeiculo(_VeiculoConfiguracao);
            _VeiculoConfiguracao.CustoTotal += AplicaCustoCarga(_VeiculoConfiguracao);

            return Ok(_VeiculoConfiguracao);
        }

        [HttpGet]
        public ActionResult<List<VeiculoConfiguracao>> GetVeiculoConfiguracao()
        {
            return Ok(VeiculoConfiguracaoList);
        }

        [HttpGet]
        public ActionResult DeleteVeiculoConfiguracao(int id)
        {
            VeiculoConfiguracao objRemovido = VeiculoConfiguracaoList.FirstOrDefault(x => x.Id == id);

            if (objRemovido is not null)
            {
                VeiculoConfiguracaoList.Remove(objRemovido);
                return Ok("Objeto Deletado.");

            }
            else
                return BadRequest();
        }

        [HttpPost]
        public ActionResult UpdateVeiculoConfiguracao([FromBody] VeiculoConfiguracao _VeiculoAtualizar)
        {
            VeiculoConfiguracao obj = VeiculoConfiguracaoList.FirstOrDefault(x => x.Id == _VeiculoAtualizar.Id);
            if (obj is null)
                return BadRequest();

            VeiculoConfiguracaoList.Remove(obj);

            _VeiculoAtualizar.CustoTotal = AplicaCustoPavimentacao(_VeiculoAtualizar);
            AplicaCustoTipoVeiculo(_VeiculoAtualizar);
            _VeiculoAtualizar.CustoTotal += AplicaCustoCarga(_VeiculoAtualizar);

            VeiculoConfiguracaoList.Add(_VeiculoAtualizar);

            return Ok(_VeiculoAtualizar);


        }

        private float AplicaCustoPavimentacao(VeiculoConfiguracao _VeiculoConfiguracao)
        {
            float custoTotal = 0;

            if (_VeiculoConfiguracao.DistanciaRodiviaPavimentada > 0)
            {
                custoTotal = _VeiculoConfiguracao.DistanciaRodiviaPavimentada * (float)0.63;
            }
            if (_VeiculoConfiguracao.DistanciaRodoviaNaoPavimentada > 0)
            {
                custoTotal += _VeiculoConfiguracao.DistanciaRodoviaNaoPavimentada * (float)0.72;
            }

            return custoTotal;
        }
        private void AplicaCustoTipoVeiculo(VeiculoConfiguracao _VeiculoConfiguracao)
        {
            TipoVeiculo TipoVeiculo = TipoVeiculoList.FirstOrDefault(x => x.NomeTipoVeiculo == _VeiculoConfiguracao.TipoVeiculoUtilizado.NomeTipoVeiculo);
            if (TipoVeiculo is null)
                return;

            _VeiculoConfiguracao.CustoTotal = _VeiculoConfiguracao.CustoTotal * TipoVeiculo.FatorMultiplicacao;

        }
        private float AplicaCustoCarga(VeiculoConfiguracao _VeiculoConfiguracao)
        {
            float custoTotal = 0;

            if (_VeiculoConfiguracao.Carga <= 5)
                return custoTotal;

            float distanciaTotal = (_VeiculoConfiguracao.DistanciaRodiviaPavimentada + _VeiculoConfiguracao.DistanciaRodoviaNaoPavimentada);
            float ValorCargaExcedente = (_VeiculoConfiguracao.Carga - 5) * (float)0.03;

            custoTotal = ValorCargaExcedente * distanciaTotal;
            return custoTotal;

        }
    }
}
