using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace WordsAndCats
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {


            string url = "http://teste.way2.com.br/dic/api/words/";
            Console.WriteLine("Informe nome desejado: ");
            string nome = Console.ReadLine().ToUpper();

            bool flagConcluido = false;
            int indice = 1;
            int contador = 1;
            while (!flagConcluido)
            {

                string palavraRetornada = LeituraWebService(url + indice);

                if (palavraRetornada == nome)
                {
                    //Encontrou
                    Console.WriteLine("Palavra encontrada com " + indice + " gatinhas mortos");
                    Console.WriteLine("Pressione qualquer tecla para encerrar o programa!");
                    Console.ReadKey();
                    flagConcluido = true;
                }
                else
                {
                    for (int i = 0; i < palavraRetornada.Length; i++)
                    {
                        if (nome[i] != palavraRetornada[i])
                        {
                            while (nome[i] != palavraRetornada[i])
                            {
                                string palavraRetornadaindice = VerificarLetras(i, nome, palavraRetornada, indice, url);
                                palavraRetornada = palavraRetornadaindice.Split('|')[0];
                                indice = int.Parse( palavraRetornadaindice.Split('|')[1]);
                            }
                        }
                        else
                        {

                        }


                    }
                }

            }


        }

        private static string VerificarLetras(int i, string nome, string palavraRetornada, int contador, string url)
        {
            //As duas palavras nao tem a primeira ou proximas letras iguais
            //Verificando se a primeira letra retornada é maior ou menor;

            int posicaoLetraWebService = PesquisarLetraAlfabeto((char)palavraRetornada[i]);
            int posicaoLetraDigitada = PesquisarLetraAlfabeto((char)nome[i]);


            int indice = 0;
            
            string palavraRetornadaAuxiliar = "";
            if (posicaoLetraDigitada > posicaoLetraWebService)
            {
                //Letra antes da posicao >>> Continuar mais a frente
                indice = contador * 10;
                palavraRetornadaAuxiliar = LeituraWebService(url + indice);
                contador++;

            }
            else
            {
                int primeiraPosicaoConhecidaLista = indice / 10;
                int UltimaPosicaoConhecidaLista = indice;

                //Letra Passou da Posicao <<< Voltar na Pesquisa
                int novaPosicao = (UltimaPosicaoConhecidaLista - primeiraPosicaoConhecidaLista) / 2;
                
                palavraRetornadaAuxiliar = LeituraWebService(url + indice);

                for (int j = 0; j < nome.Length; j++)
                {
                    try
                    {
                        while (nome[j] != palavraRetornadaAuxiliar[j])
                        {

                            //Decidir se a primeira letra é maior ou menor
                            int posicaoLetraWebServiceAux = PesquisarLetraAlfabeto((char)palavraRetornadaAuxiliar[j]);
                            int posicaoLetraDigitadaAux = PesquisarLetraAlfabeto((char)nome[j]);

                            palavraRetornadaAuxiliar = "";
                            if (posicaoLetraDigitadaAux > posicaoLetraWebServiceAux)
                            {
                                //Letra antes da posicao >>> Continuar na 2a secao
                                indice = (UltimaPosicaoConhecidaLista - novaPosicao) / 2;
                                palavraRetornadaAuxiliar = LeituraWebService(url + indice);
                                novaPosicao = indice;

                            }
                            else
                            {
                                //Letra antes da posicao <<< Continuar na 1a secao
                                indice = (novaPosicao - primeiraPosicaoConhecidaLista) / 2;
                                palavraRetornadaAuxiliar = LeituraWebService(url + indice);
                                UltimaPosicaoConhecidaLista = indice;
                                novaPosicao = indice;
                            }
                        }
                        //Segue para a proxima letra apenas qdo a 1a estiver OK
                        contador++;
                    }
                    catch
                    {
                        //O total de letras da palavra digitada excedeu o total de letras da palavra retornada
                    }
                }
                



            }

            return palavraRetornadaAuxiliar + "|" + indice;
        }

        private static int PesquisarLetraAlfabeto(char _letra)
        {
            char[] alfabeto = "ABCDEFGHKIJLMNOPQRSTUVXYZ".ToCharArray();
            int contador = 0;
            foreach (var i in alfabeto)
            {
                contador++;
                if (_letra == i)
                {
                    return contador;
                }

            }

            return 0;

        }



        private static string LeituraWebService(string _url)
        {
            try
            {

                WebRequest request = WebRequest.Create(_url);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Method = "GET";
                request.PreAuthenticate = true;
                request.ContentType = @"application/json";

                WebResponse response = request.GetResponse();
                //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                if (((HttpWebResponse)response).StatusDescription == "OK")
                {


                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    Console.WriteLine(responseFromServer);

                    reader.Close();
                    response.Close();

                    return responseFromServer.ToUpper();
                }
                else
                {
                    return "NAO ENCONTRADO";
                }


            }
            catch (Exception ex)
            {
                return "";

            }

        }

    }
}


