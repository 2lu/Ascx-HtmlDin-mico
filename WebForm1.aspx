<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebApplication1.WebForm1" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="css/bootstrap.min.css">

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <link rel="stylesheet" href="http://code.jquery.com/ui/1.9.0/themes/base/jquery-ui.css" />
    <script src="http://code.jquery.com/jquery-1.8.2.js"></script>
    <script src="http://code.jquery.com/ui/1.9.0/jquery-ui.js"></script>
        
    <style>
        div[class^="Hiden"] {display: none;}
    </style>
    <script>

    $(document).ready(function(){
        // Mostra campos obrigatorios de uma div
        $(".btn").click(function () {
            var id = $(this).parent();
            var erro = false;
            $("div[id*='" + id[0].id + "'] > div > input").each(function () {
                
                    if ($(this).val() == "" && $(this).prop('required')) {
                        erro = true;
                        $(this).css({ "border": "1px solid red" });
                    }
                    else {
                        $(this).css({ "border": "1px solid gray" });
                    }

            });

            if(!erro)
                alert(id[0].id + " Gravado com sucesso.")
           
        });
       
    });


    //Mostra campos Hiden
    function MostraControle(classe) {
        $(classe).show();
    }

    //Esconde campos Hiden
    function EscondeControle(classe) {
        $(classe).hide();
    }
    $(function() {
        $("input[id*='data']").datepicker({
            showOtherMonths: true,
            selectOtherMonths: true,
            changeMonth: true,
            changeYear: true,
            dateFormat: 'dd/mm/yy',
            dayNames: ['Domingo','Segunda','Terça','Quarta','Quinta','Sexta','Sábado','Domingo'],
            dayNamesMin: ['D','S','T','Q','Q','S','S','D'],
            dayNamesShort: ['Dom','Seg','Ter','Qua','Qui','Sex','Sáb','Dom'],
            monthNames: ['Janeiro','Fevereiro','Março','Abril','Maio','Junho','Julho','Agosto','Setembro','Outubro','Novembro','Dezembro'],
            monthNamesShort: ['Jan','Fev','Mar','Abr','Mai','Jun','Jul','Ago','Set','Out','Nov','Dez']
        });

    });

   
 </script>

</head>
<body style="width: 900px">

    <div class="container" style="max-width: 600px; margin: 60px auto;">
        <form id="FormDinamico" runat="server" role="form" class="form-horizontal">
            <div class="panel-group">
                <asp:PlaceHolder ID="FormularioDinamico" runat="server"></asp:PlaceHolder>
            </div>
           
        </form>
    </div>
</body>
</html>
