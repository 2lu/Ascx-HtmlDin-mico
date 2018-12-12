using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace WebApplication1
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        static HttpClient client;

        protected void Page_Load(object sender, EventArgs e)
        {
            FomularioDinamico();
        }
		//Cofigura o Client
        public WebForm1()
        {
            if (client == null)
            {
                client = new HttpClient();

                client.BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["ApiGetCamposFormulario"].ToString());
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

		// Busca array Json
        private string ObterFormulario(HttpRequest Request)
        {

            try
            {
                //chamando a api pela url
                //resultado = client.GetStringAsync("api/CadastroUnico/ConsultaCamposFormulario").Result;
				return '[{"Entidade": "Beneficiário","Nome": "Nome", "NomeExibicao": "Nome Completo", "Tipo": "String", "Obrigatorio": "required",  "CodigoCampo": 1}, {"Entidade": "Beneficiário","Nome": "Nascimento", "NomeExibicao": "Data de Nascimento","Tipo": "DateTime", "Obrigatorio": "required","CodigoCampo": 2},{"Entidade": "Paciente","Nome": "Sexo", "NomeExibicao": "Sexo", "Tipo": "Select", "Obrigatorio": "required", "CodigoCampo": 3,"Opcao": [{"Valor": "F","Texto": "Feminino"},{"Valor": "M", "Texto": "Masculino"}]},{"Entidade": "Paciente","Nome": "Titular","NomeExibicao": "Paciente é titular?", "Tipo": "Boolean", "Obrigatorio": "required","CodigoCampo": 19},{"Entidade": "Produto","Nome": "TipoConselho", "NomeExibicao": "Tipo de conselho","Tipo": "Select","Obrigatorio": "required", "CodigoCampo": 21},{ "Entidade": "Produto", "Nome": "MotivoInscricaoPrograma", "NomeExibicao": "Motivo para inscrição no programa","Tipo": "Boolean", "Obrigatorio": "required", "ValorInicial": "false","ProdutoQuestaoAssociada": 23, "CodigoCampo": 28,"Opcao": [{ "Valor": "N", "Texto": "Não"},{ "Valor": "S", "Texto": "Sim"} ]}]'
            }
            catch
            { throw; }
        }



        enum enumValorCampos
        {
            [Description("Entidade")]
            Entidade,
            [Description("Nome")]
            Nome,
            [Description("NomeExibicao")]
            NomeExibicao,
            [Description("Tipo")]
            Tipo,
            [Description("Obrigatorio")]
            Obrigatorio,
            [Description("ValorInicial")]
            ValorInicial,
            [Description("ProdutoQuestaoAssociada")]
            ProdutoQuestaoAssociada,
            [Description("CodigoCampo")]
            CodigoCampo,
            [Description("Opcao")]
            Opcao,
            [Description("Valor")]
            Valor,
            [Description("Texto")]
            Texto
        }

        enum enumEntidade
        {
            [Description("Beneficiário")]
            Beneficiario,
            [Description("Paciente")]
            Paciente,
            [Description("Produto")]
            Produto,
            [Description("Outros")]
            Outros
        }

        public void FomularioDinamico()
        {
            try
            {
                bool imprimediv = true;
                string Entidade = null;
                object LayoutForm = JsonConvert.DeserializeObject(ObterFormulario(Request).ToString());

                IList ListaCampos = ((IEnumerable<object>)LayoutForm).ToList();
                HtmlGenericControl div = null;
                HtmlGenericControl div2;

                if (ListaCampos.Count > 0)
                {

                    foreach (JToken rowOjb in ListaCampos)
                    {

                        string FieldName = (string)rowOjb[enumValorCampos.NomeExibicao.GetDescription()];
                        string FieldType = (string)rowOjb[enumValorCampos.Tipo.GetDescription()];

                        imprimediv = Entidade != (string)rowOjb[enumValorCampos.Entidade.GetDescription()] ? true : false;

                        // Se nova Div (Entidade)
                        if (imprimediv)
                        {

                            if(!string.IsNullOrEmpty(Entidade))  ImprimiBotao(div, "Incluir");
                            Entidade = (string)rowOjb[enumValorCampos.Entidade.GetDescription()];
                            div = new HtmlGenericControl("div");
                            div.ID = Entidade;
                            div.Attributes.Add("class", "panel panel-default");
                            div2 = new HtmlGenericControl("div");
                            div2.Attributes.Add("class", "panel-heading");
                            div2.InnerText = Entidade;
                            div.Controls.Add(div2);
                        }

                        div2 = new HtmlGenericControl("div");
                        Label lbcustomename = new Label();
                        lbcustomename.ID = "lb" + (string)rowOjb[enumValorCampos.Nome.GetDescription()];
                        lbcustomename.Text = FieldName + ": ";

                        div2.Attributes.Add("class", "panel-heading");
                        div2.Controls.Add(lbcustomename);

                        if (rowOjb[enumValorCampos.Tipo.GetDescription()].ToString().ToLower() == "string" || FieldType.ToLower().Trim() == "integer" || FieldType.ToLower().Trim() == "decimal" || FieldType.ToLower().Trim() == "datetime")
                        {
                            TextBox txtcustombox = new TextBox();

                            txtcustombox.ID = FieldType.ToLower().Trim() == "string" ? "txt" + FieldName : FieldType.ToLower().Trim() == "datetime" ? "data" + (string)rowOjb[enumValorCampos.Nome.GetDescription()] : "nr" + FieldName;
                            txtcustombox.Attributes.Add("runat", "server");
                            txtcustombox.Attributes.Add("class", "form-control");
                            if (!string.IsNullOrWhiteSpace((string)rowOjb[enumValorCampos.Obrigatorio.GetDescription()])) txtcustombox.Attributes.Add((string)rowOjb[enumValorCampos.Obrigatorio.GetDescription()], "");

                            // Ha valor inicial default
                            if (!string.IsNullOrEmpty((string)rowOjb[enumValorCampos.ValorInicial.ToString()]))
                                txtcustombox.Text = ((string)rowOjb[enumValorCampos.ValorInicial.GetDescription()]);


                            // Campo possuem ProdutoQuestaoAssociada ?
                            if (!string.IsNullOrEmpty((string)rowOjb[enumValorCampos.ProdutoQuestaoAssociada.ToString()]))
                                AssociaQuestaoCampo(ref div2, ref txtcustombox, rowOjb);

                            div2.Controls.Add(txtcustombox);

                        }

                        else if (rowOjb[enumValorCampos.Tipo.GetDescription()].ToString().ToLower() == "boolean")
                        {
                            RadioButtonList rbnlst = new RadioButtonList();
                            rbnlst.ID = "rbnlst" + (string)rowOjb[enumValorCampos.Nome.GetDescription()];
                            rbnlst.Attributes.Add("runat", "server");
                            if (!string.IsNullOrWhiteSpace((string)rowOjb[enumValorCampos.Obrigatorio.GetDescription()])) rbnlst.Attributes.Add((string)rowOjb[enumValorCampos.Obrigatorio.GetDescription()], "");
                            rbnlst.Items.Add(new ListItem("Sim", "1"));
                            rbnlst.Items.Add(new ListItem("Não", "2"));
                            // Ha valor inicial default
                            if (!string.IsNullOrEmpty((string)rowOjb[enumValorCampos.ValorInicial.ToString()]))
                                rbnlst.SelectedValue = ((Boolean)rowOjb[enumValorCampos.ValorInicial.GetDescription()]) ? "1" : "2";
                            else
                                rbnlst.SelectedValue = "1";

                            // Campo possuem ProdutoQuestaoAssociada ?
                            if (!string.IsNullOrEmpty((string)rowOjb[enumValorCampos.ProdutoQuestaoAssociada.ToString()]))
                                AssociaQuestaoCampo(ref div2, ref rbnlst, rowOjb);

                            div2.Controls.Add(rbnlst);
                        }
                        else if (rowOjb[enumValorCampos.Nome.GetDescription()].ToString().ToLower() == "sexo")
                        {
                            RadioButtonList rbnlst = new RadioButtonList();
                            rbnlst.ID = "rbnlst" + (string)rowOjb[enumValorCampos.Nome.GetDescription()];
                            rbnlst.Attributes.Add("runat", "server");
                            if (!string.IsNullOrWhiteSpace((string)rowOjb[enumValorCampos.Obrigatorio.GetDescription()])) rbnlst.Attributes.Add((string)rowOjb[enumValorCampos.Obrigatorio.GetDescription()], "");
                            rbnlst.Items.Add(new ListItem("Feminino", "F"));
                            rbnlst.Items.Add(new ListItem("Masculino", "M"));
                            // Ha valor inicial default
                            if (!string.IsNullOrEmpty((string)rowOjb[enumValorCampos.ValorInicial.ToString()]))
                                rbnlst.SelectedValue = ((Boolean)rowOjb[enumValorCampos.ValorInicial.GetDescription()]) ? "1" : "2";
                            else
                                rbnlst.SelectedValue = "1";

                            rbnlst.RepeatDirection = RepeatDirection.Horizontal;
                            div2.Controls.Add(rbnlst);


                        }

                        else if (rowOjb[enumValorCampos.Nome.ToString()].ToString().ToLower() == "uf")
                        {
                            DropDownList ddllst = new DropDownList();
                            ddllst.Attributes.Add("runat", "server");
                            ddllst.Attributes.Add("class", "form-control");
                            if (!string.IsNullOrWhiteSpace((string)rowOjb[enumValorCampos.Obrigatorio.GetDescription()])) ddllst.Attributes.Add((string)rowOjb[enumValorCampos.Obrigatorio.GetDescription()], "");
                            ddllst.ID = "ddl" + (string)rowOjb[enumValorCampos.Nome.GetDescription()];
                            ddllst.Items.Add(new ListItem("Select", "0"));
                            ddllst.Items.Add(new ListItem("São Paulo", "SP"));
                            ddllst.Items.Add(new ListItem("Rio de janeiro", "RJ"));
                            // Ha valor inicial default
                            if (!string.IsNullOrEmpty((string)rowOjb[enumValorCampos.ValorInicial.ToString()]))
                                ddllst.SelectedValue = (string)rowOjb[enumValorCampos.ValorInicial.GetDescription()];
                            else
                                ddllst.SelectedValue = "0";
                            div2.Controls.Add(ddllst);


                        }
                        else if (rowOjb[enumValorCampos.Tipo.ToString()].ToString().ToLower() == "select")
                        {
                            DropDownList ddllst = new DropDownList();
                            ddllst.Attributes.Add("runat", "server");
                            ddllst.Attributes.Add("class", "form-control");
                            if (!string.IsNullOrWhiteSpace((string)rowOjb[enumValorCampos.Obrigatorio.GetDescription()])) ddllst.Attributes.Add((string)rowOjb[enumValorCampos.Obrigatorio.GetDescription()], "");
                            ddllst.ID = "ddl" + (string)rowOjb[enumValorCampos.Nome.GetDescription()];
                            ddllst.Items.Add(new ListItem("Select", "0"));
                            // Se dropdow possuem campo associado a questao
                            if (!string.IsNullOrEmpty((string)rowOjb[enumValorCampos.ProdutoQuestaoAssociada.ToString()]))
                                ddllst.Items[0].Attributes.Add("Onclick", "EscondeControle('.Hiden" + (string)rowOjb[enumValorCampos.CodigoCampo.ToString()] + "')");

                            // Se houver lista  Select monta combo de lista
                            if (rowOjb[enumValorCampos.Opcao.ToString()] != null)
                            {
                                IList Opcao = rowOjb[enumValorCampos.Opcao.ToString()].ToArray();
                                foreach (JToken row in Opcao)
                                {
                                    ddllst.Items.Add(new ListItem((string)row[enumValorCampos.Texto.ToString()], (string)row[enumValorCampos.Valor.ToString()]));
                                }
                            }
                            // Se não houver uma lista - Select
                            else
                                ddllst.Items.Add(new ListItem("Os itens da listas " + FieldName + " não existe no objeto Json."));

                            if (!string.IsNullOrEmpty((string)rowOjb[enumValorCampos.ValorInicial.ToString()]))
                                ddllst.SelectedValue = (string)rowOjb[enumValorCampos.ValorInicial.GetDescription()];
                            else
                                ddllst.SelectedValue = "0";

                                // Campo possuem ProdutoQuestaoAssociada ?
                            if (!string.IsNullOrEmpty((string)rowOjb[enumValorCampos.ProdutoQuestaoAssociada.ToString()]))
                                    AssociaQuestaoCampo(ref div2, ref ddllst, rowOjb);

                            div2.Controls.Add(ddllst);

                        }

                        div.Controls.Add(div2);

                        if (imprimediv) FormularioDinamico.Controls.Add(div);

                    }
                    FormularioDinamico.Controls.Add(div);
                    if (Entidade != enumEntidade.Paciente.GetDescription()) ImprimiBotao(div, "Incluir");

                }

            }
            catch (Exception ex)
            {
                do
                {
                    Response.Write("<br>" + ex.Message);
                    ex = ex.InnerException;
                }
                while (ex != null);

            }
        }

        void ImprimiBotao(HtmlGenericControl div, string descricao)
        {

            if (descricao == "Incluir")
            {
                HtmlInputButton btn = new HtmlInputButton();
                btn.ID = "btn" + descricao + div.ID;
                btn.Attributes.Add("onclick", "Alert('" + div.ID + " gravado com sucesso.');");
                btn.Value = descricao;
                btn.Attributes.Add("class", "btn btn-primary");
                div.Controls.Add(btn);
            }
            else if (descricao == "Gravar")
            {
                Button btn = new Button();
                btn.ID = "btn" + descricao + div.ID;
                btn.OnClientClick = "Alert('Dados gravados com sucesso');";
                btn.Text = descricao;
                btn.Attributes.Add("class", "btn btn-primary");
                div.Controls.Add(btn);
            }

            FormularioDinamico.Controls.Add(div);

        }

        void AssociaQuestaoCampo(ref HtmlGenericControl div, ref RadioButtonList campoFormulario, JToken arrayJson)
        {

            if ((int)arrayJson[enumValorCampos.ProdutoQuestaoAssociada.GetDescription()] == (int)arrayJson[enumValorCampos.CodigoCampo.GetDescription()])
                campoFormulario.Attributes.Add("Onclick", "MostraControle('.Hiden" + (string)arrayJson[enumValorCampos.CodigoCampo.ToString()] + "')");

            //E o campo a ser escondido?    
            else
                div.Attributes.Add("class", "Hiden" + (string)arrayJson[enumValorCampos.ProdutoQuestaoAssociada.ToString()] + " panel-heading");
        }

        void AssociaQuestaoCampo(ref HtmlGenericControl div, ref DropDownList campoFormulario, JToken arrayJson)
        {

            if ((int)arrayJson[enumValorCampos.ProdutoQuestaoAssociada.GetDescription()] == (int)arrayJson[enumValorCampos.CodigoCampo.GetDescription()])
                campoFormulario.Attributes.Add("Onclick", "MostraControle('.Hiden" + (string)arrayJson[enumValorCampos.CodigoCampo.ToString()] + "')");

            //E o campo a ser escondido?    
            else
                div.Attributes.Add("class", "Hiden" + (string)arrayJson[enumValorCampos.ProdutoQuestaoAssociada.ToString()] + " panel-heading");
        }

        void AssociaQuestaoCampo(ref HtmlGenericControl div, ref TextBox campoFormulario, JToken arrayJson)
        {

            if ((int)arrayJson[enumValorCampos.ProdutoQuestaoAssociada.GetDescription()] == (int)arrayJson[enumValorCampos.CodigoCampo.GetDescription()])
                campoFormulario.Attributes.Add("Onclick", "MostraControle('.Hiden" + (string)arrayJson[enumValorCampos.CodigoCampo.ToString()] + "')");

            //E o campo a ser escondido?    
            else
                div.Attributes.Add("class", "Hiden" + (string)arrayJson[enumValorCampos.ProdutoQuestaoAssociada.ToString()] + " panel-heading");
        }

    }
  
}