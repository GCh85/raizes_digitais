package pt.raizesdigitais.app

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.RequestBody.Companion.toRequestBody
import org.json.JSONObject

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun IaNarrativaScreen(qrId: Int, tituloQr: String, onVoltar: () -> Unit) {
    val scope = rememberCoroutineScope()
    var narrativa by remember { mutableStateOf("") }
    var estado by remember { mutableStateOf("loading") }

    LaunchedEffect(qrId) {
        scope.launch {
            estado = "loading"
            try {
                val idCliente = SessaoManager.obterIdCliente()
                narrativa = gerarNarrativaViaServidor(idCliente, qrId)
                estado = "ok"
                try {
                    registarPontosQr(idCliente, qrId)
                } catch (_: Exception) { }
            } catch (e: Exception) {
                narrativa = e.message ?: "erro desconhecido"
                estado = "erro"
            }
        }
    }

    Column(modifier = Modifier.fillMaxSize()) {
        TopAppBar(
            title = { },
            navigationIcon = {
                IconButton(onClick = onVoltar) {
                    Icon(
                        Icons.AutoMirrored.Filled.ArrowBack,
                        contentDescription = "Voltar",
                        tint = MaterialTheme.colorScheme.onBackground
                    )
                }
            },
            colors = TopAppBarDefaults.topAppBarColors(
                containerColor = MaterialTheme.colorScheme.background
            )
        )

        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(horizontal = 20.dp)
                .verticalScroll(rememberScrollState()),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Text(
                text = tituloQr,
                style = MaterialTheme.typography.headlineLarge,
                color = MaterialTheme.colorScheme.onBackground,
                textAlign = TextAlign.Center,
                modifier = Modifier.padding(bottom = 24.dp)
            )

            when (estado) {
                "loading" -> {
                    Spacer(modifier = Modifier.height(40.dp))
                    CircularProgressIndicator(
                        color = MaterialTheme.colorScheme.primary,
                        strokeWidth = 3.dp
                    )
                    Spacer(modifier = Modifier.height(24.dp))
                    Text(
                        text = "A preparar a sua historia...",
                        style = MaterialTheme.typography.bodyLarge,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                }
                "ok" -> {
                    Card(
                        modifier = Modifier.fillMaxWidth(),
                        colors = CardDefaults.cardColors(
                            containerColor = MaterialTheme.colorScheme.primaryContainer.copy(alpha = 0.2f)
                        ),
                        shape = MaterialTheme.shapes.small
                    ) {
                        Text(
                            text = narrativa,
                            style = MaterialTheme.typography.bodyLarge,
                            textAlign = TextAlign.Justify,
                            color = MaterialTheme.colorScheme.onBackground,
                            modifier = Modifier.padding(20.dp)
                        )
                    }

                    Spacer(modifier = Modifier.height(24.dp))

                    Text(
                        text = "Narrativa criada por IA com base no seu perfil.",
                        style = MaterialTheme.typography.labelSmall,
                        color = MaterialTheme.colorScheme.onSurfaceVariant,
                        textAlign = TextAlign.Center
                    )

                    Spacer(modifier = Modifier.height(32.dp))
                }
                "erro" -> {
                    Spacer(modifier = Modifier.height(40.dp))
                    Card(
                        modifier = Modifier.fillMaxWidth(),
                        colors = CardDefaults.cardColors(
                            containerColor = MaterialTheme.colorScheme.errorContainer.copy(alpha = 0.3f)
                        ),
                        shape = MaterialTheme.shapes.small
                    ) {
                        Text(
                            text = narrativa,
                            textAlign = TextAlign.Center,
                            color = MaterialTheme.colorScheme.error,
                            modifier = Modifier.padding(20.dp)
                        )
                    }
                }
            }
        }
    }
}

private suspend fun gerarNarrativaViaServidor(idCliente: Int, idQr: Int): String =
    withContext(Dispatchers.IO) {
        val client = OkHttpClient()
        val body = JSONObject().apply {
            put("id_cliente", idCliente)
            put("id_qr", idQr)
        }.toString()

        val request = Request.Builder()
            .url("${Constants.BASE_URL}/narrativa_ia.ashx")
            .post(body.toRequestBody("application/json".toMediaType()))
            .header("ngrok-skip-browser-warning", "true")
            .build()

        val response = client.newCall(request).execute().use { it.body!!.string() }
        val json = JSONObject(response)

        if (json.optBoolean("sucesso", false)) {
            json.getString("narrativa")
        } else {
            throw Exception(json.optString("erro", "Erro ao gerar narrativa"))
        }
    }

private suspend fun registarPontosQr(idCliente: Int, idQr: Int) =
    withContext(Dispatchers.IO) {
        val client = OkHttpClient()
        val body = org.json.JSONObject().apply {
            put("id_cliente", idCliente)
            put("id_qr", idQr)
        }.toString()
        val request = Request.Builder()
            .url("${Constants.BASE_URL}/qr_pontos.ashx")
            .post(body.toRequestBody("application/json".toMediaType()))
            .header("ngrok-skip-browser-warning", "true")
            .build()
        client.newCall(request).execute().close()
    }