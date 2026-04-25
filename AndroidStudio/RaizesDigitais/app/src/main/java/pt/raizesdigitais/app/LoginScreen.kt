package pt.raizesdigitais.app

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.RequestBody.Companion.toRequestBody
import org.json.JSONObject

data class LoginResposta(
    val sucesso: Boolean,
    val id_cliente: Int = 0,
    val nome: String = "",
    val email: String = "",
    val erro: String = ""
)

@Composable
fun LoginScreen(onLoginSucesso: () -> Unit) {
    var email by remember { mutableStateOf("") }
    var password by remember { mutableStateOf("") }
    var loading by remember { mutableStateOf(false) }
    var erro by remember { mutableStateOf("") }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(horizontal = 24.dp, vertical = 32.dp),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        // Logo ou título
        Text(
            text = "Raízes Digitais",
            style = MaterialTheme.typography.headlineLarge,
            color = MaterialTheme.colorScheme.primary
        )

        Spacer(modifier = Modifier.height(8.dp))

        Text(
            text = "A sua aventura na Quinta da Azenha",
            style = MaterialTheme.typography.bodyMedium,
            color = MaterialTheme.colorScheme.onSurfaceVariant,
            textAlign = TextAlign.Center
        )

        Spacer(modifier = Modifier.height(48.dp))

        // Campo Email
        OutlinedTextField(
            value = email,
            onValueChange = { email = it },
            label = { Text("Email") },
            singleLine = true,
            keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Email),
            modifier = Modifier.fillMaxWidth(),
            colors = OutlinedTextFieldDefaults.colors(
                focusedBorderColor = MaterialTheme.colorScheme.primary,
                unfocusedBorderColor = MaterialTheme.colorScheme.outline
            )
        )

        Spacer(modifier = Modifier.height(16.dp))

        // Campo Password
        OutlinedTextField(
            value = password,
            onValueChange = { password = it },
            label = { Text("Password") },
            singleLine = true,
            visualTransformation = PasswordVisualTransformation(),
            keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Password),
            modifier = Modifier.fillMaxWidth(),
            colors = OutlinedTextFieldDefaults.colors(
                focusedBorderColor = MaterialTheme.colorScheme.primary,
                unfocusedBorderColor = MaterialTheme.colorScheme.outline
            )
        )

        Spacer(modifier = Modifier.height(24.dp))

        // Botão Login
        Button(
            onClick = {
                if (email.isBlank() || password.isBlank()) {
                    erro = "Preencha todos os campos"
                    return@Button
                }
                loading = true
                erro = ""
                Thread {
                    val resultado = fazerLogin(email, password)
                    loading = false
                    if (resultado.sucesso) {
                        SessaoManager.guardarSessao(
                            resultado.id_cliente,
                            resultado.nome,
                            resultado.email
                        )
                        onLoginSucesso()
                    } else {
                        erro = resultado.erro
                    }
                }.start()
            },
            enabled = !loading,
            modifier = Modifier
                .fillMaxWidth()
                .height(56.dp),
            shape = MaterialTheme.shapes.small,
            colors = ButtonDefaults.buttonColors(
                containerColor = MaterialTheme.colorScheme.primary
            )
        ) {
            if (loading) {
                CircularProgressIndicator(
                    color = MaterialTheme.colorScheme.onPrimary,
                    strokeWidth = 2.dp,
                    modifier = Modifier.size(24.dp)
                )
            } else {
                Text("Entrar", style = MaterialTheme.typography.labelLarge)
            }
        }

        // Mensagem de erro
        if (erro.isNotEmpty()) {
            Spacer(modifier = Modifier.height(16.dp))
            Text(
                text = erro,
                color = MaterialTheme.colorScheme.error,
                style = MaterialTheme.typography.bodyMedium,
                textAlign = TextAlign.Center
            )
        }

        Spacer(modifier = Modifier.weight(1f))

        // Nota informativa
        Text(
            text = "Use as credenciais da sua conta no site da Quinta da Azenha",
            style = MaterialTheme.typography.bodySmall,
            color = MaterialTheme.colorScheme.onSurfaceVariant,
            textAlign = TextAlign.Center
        )
    }
}

private fun fazerLogin(email: String, password: String): LoginResposta {
    return try {
        val client = OkHttpClient()
        val jsonBody = JSONObject().apply {
            put("email", email)
            put("password", password)
        }.toString()

        val request = Request.Builder()
            .url("${Constants.BASE_URL}/login_cliente.ashx")
            .post(jsonBody.toRequestBody("application/json".toMediaType()))
            .header("ngrok-skip-browser-warning", "true")
            .build()

        val response = client.newCall(request).execute()
        val responseBody = response.body?.string() ?: "{}"
        val json = JSONObject(responseBody)

        if (json.optBoolean("sucesso", false)) {
            LoginResposta(
                sucesso = true,
                id_cliente = json.getInt("id_cliente"),
                nome = json.optString("nome", ""),
                email = json.optString("email", email)
            )
        } else {
            LoginResposta(
                sucesso = false,
                erro = json.optString("erro", "Credenciais inválidas")
            )
        }
    } catch (e: Exception) {
        LoginResposta(
            sucesso = false,
            erro = "Erro de ligação: ${e.message}"
        )
    }
}