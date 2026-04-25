package pt.raizesdigitais.app

import android.content.Context
import android.content.SharedPreferences

object SessaoManager {
    private const val PREFS_NAME = "raizes_digitais_session"
    private const val KEY_ID_CLIENTE = "id_cliente"
    private const val KEY_NOME_CLIENTE = "nome_cliente"
    private const val KEY_EMAIL_CLIENTE = "email_cliente"

    private lateinit var prefs: SharedPreferences

    fun init(context: Context) {
        prefs = context.getSharedPreferences(PREFS_NAME, Context.MODE_PRIVATE)
    }

    fun guardarSessao(idCliente: Int, nome: String, email: String) {
        prefs.edit()
            .putInt(KEY_ID_CLIENTE, idCliente)
            .putString(KEY_NOME_CLIENTE, nome)
            .putString(KEY_EMAIL_CLIENTE, email)
            .apply()
    }

    fun obterIdCliente(): Int = prefs.getInt(KEY_ID_CLIENTE, 0)

    fun obterNomeCliente(): String = prefs.getString(KEY_NOME_CLIENTE, "") ?: ""

    fun obterEmailCliente(): String = prefs.getString(KEY_EMAIL_CLIENTE, "") ?: ""

    fun temSessao(): Boolean = obterIdCliente() > 0

    fun limparSessao() {
        prefs.edit().clear().apply()
    }

    private const val KEY_AVALIACOES_SESSAO = "avaliacoes_sessao"

    fun obterAvaliacoesSessao(): Int = prefs.getInt(KEY_AVALIACOES_SESSAO, 0)

    fun incrementarAvaliacoesSessao() {
        prefs.edit().putInt(KEY_AVALIACOES_SESSAO, obterAvaliacoesSessao() + 1).apply()
    }

    fun resetarAvaliacoesSessao() {
        prefs.edit().putInt(KEY_AVALIACOES_SESSAO, 0).apply()
    }

}