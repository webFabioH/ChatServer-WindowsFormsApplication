﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer
{
    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);

    internal class Servidor
    {
        public static Hashtable htUsuarios = new Hashtable(30);
        public static Hashtable htConexoes = new Hashtable(30);
        private IPAddress enderecoIP;
        private int portaHost;
        private TcpClient tcpCliente;

        public static event StatusChangedEventHandler StatusChanged;
        private static StatusChangedEventArgs e;

        public Servidor(IPAddress endereco, int porta) 
        {
            enderecoIP = endereco;
            portaHost = porta;
        }

        private Thread thrListener;
        private TcpListener tlsCliente;

        bool ServerRodando = false;

        public static void IncluirUsuario(TcpClient tcpUsuario, string strUsername) 
        {
            Servidor.htUsuarios.Add(strUsername, tcpUsuario);
            Servidor.htConexoes.Add(tcpUsuario, strUsername);

            EnviaMensagemAdmin(htConexoes[tcpUsuario] + " entrou...");
        }

        public static void RemoveUsuario(TcpClient tcpUsuario)
        {
            if (htConexoes[tcpUsuario] != null)
            {
                EnviaMensagemAdmin(htConexoes[tcpUsuario] + " saiu...");

                Servidor.htUsuarios.Remove(Servidor.htConexoes[tcpUsuario]);
                Servidor.htConexoes.Remove(tcpUsuario);
            }
        }

        public static void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChangedEventHandler statusHandler = StatusChanged;

            if (statusHandler != null) 
            {
                statusHandler(null, e);
            }
        }

        public static void EnviaMensagemAdmin(string Mensagem)
        {
            StreamWriter swSenderSender;

            e = new StatusChangedEventArgs("Administrador: " + Mensagem);
            OnStatusChanged(e);

            TcpClient[] tcpClientes = new TcpClient[Servidor.htUsuarios.Count];
            Servidor.htUsuarios.Values.CopyTo(tcpClientes, 0);

            for (int i = 0; i < tcpClientes.Length; i++)
            {
                try
                {
                    if (Mensagem.Trim() == "" || tcpClientes[i] == null)
                    {
                        continue;
                    }

                    swSenderSender = new StreamWriter(tcpClientes[i].GetStream());
                    swSenderSender.WriteLine("Administrador: " + Mensagem);
                    swSenderSender.Flush();
                    swSenderSender = null;
                }
                catch 
                {
                    RemoveUsuario(tcpClientes[i]);
                }
            }
        }

        public static void EnviaMensagem(string Origem, string Mensagem)
        {
            StreamWriter swSenderSender;

            e = new StatusChangedEventArgs(Origem + " disse: " + Mensagem);
            OnStatusChanged(e);

            TcpClient[] tcpClientes = new TcpClient[Servidor.htUsuarios.Count];
            Servidor.htUsuarios.Values.CopyTo(tcpClientes, 0);

            for (int i = 0; i < tcpClientes.Length; i++)
            {
                try 
                { 
                    if (Mensagem.Trim() == "" || tcpClientes[i] == null)
                    {
                        continue;
                    }

                    swSenderSender = new StreamWriter(tcpClientes[i].GetStream());
                    swSenderSender.WriteLine(Origem + " disse: " + Mensagem);
                    swSenderSender.Flush();
                    swSenderSender = null;
                }
                catch
                {
                    RemoveUsuario(tcpClientes[i]);
                }
            }
        }

        public void InicaAtendimento()
        {
            try
            {
                IPAddress ipaLocal;
                ipaLocal = enderecoIP;
                int portaLocal = portaHost;

                tlsCliente = new TcpListener(ipaLocal, portaLocal);

                tlsCliente.Start();

                ServerRodando = true;

                thrListener = new Thread(MantemAtendimento);
                thrListener.IsBackground = true;
                thrListener.Start();
            }
            catch (Exception ex)
            {

            }
        }

        private void MantemAtendimento()
        {
            while (ServerRodando)
            {
                tcpCliente = tlsCliente.AcceptTcpClient();

                Conexao newConnection = new Conexao(tcpCliente);
            }
        }
    }
}
