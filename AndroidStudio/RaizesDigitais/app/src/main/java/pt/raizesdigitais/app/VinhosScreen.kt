package pt.raizesdigitais.app

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Star
import androidx.compose.material.icons.outlined.StarOutline
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.style.TextOverflow
import androidx.compose.ui.unit.dp
import androidx.compose.ui.window.Dialog
import com.google.gson.Gson
import com.google.gson.reflect.TypeToken
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.RequestBody.Companion.toRequestBody
import org.json.JSONObject

data class Vinho(
    val id_vinho: Int,
    val nome: String,
    val casta: String?,
    val ano: Int?,
    val preco: Double,
    val tipo: String?,
    val descricao: String?,
    val imagem_url: String?,
    val ja_avaliou: Boolean = false,
    val media_estrelas: Double = 0.0,
    val total_avaliacoes: Int = 0
)

@Composable
fun VinhosScreen(idCliente: Int, onVoltar: () -> Unit) {
    var vinhos by remember { mutableStateOf<List<Vinho>>(emptyList()) }
    var loading by remember { mutableStateOf(true) }
    var erro by remember { mutableStateOf("") }
    var vinhoParaAvaliar by remember { mutableStateOf<Vinho?>(null) }
    var snackMessage by remember { mutableStateOf("") }
    val snackbarHostState = remember { SnackbarHostState() }

    fun carregarVinhos() {
        Thread {
            try {
                val client = OkHttpClient()
                val request = Request.Builder()
                    .url("${Constants.BASE_URL}/listar_vinhos.ashx?id_cliente=$idCliente")
                    .header("ngrok-skip-browser-warning", "true")
                    .build()
                val response = client.newCall(request).execute()
                val json = response.body?.string() ?: "{}"
                val obj = JSONObject(json)
                if (obj.optBoolean("sucesso", false)) {
                    val tipo = object : TypeToken<List<Vinho>>() {}.type
                    vinhos = Gson().fromJson(obj.getString("vinhos"), tipo)
                } else {
                    erro = obj.optString("erro", "Erro ao carregar vinhos")
                }
            } catch (e: Exception) {
                erro = e.message ?: "Erro desconhecido"
            } finally {
                loading = false
            }
        }.start()
    }

    LaunchedEffect(Unit) { carregarVinhos() }

    LaunchedEffect(snackMessage) {
        if (snackMessage.isNotEmpty()) {
            snackbarHostState.showSnackbar(snackMessage)
            snackMessage = ""
        }
    }

    // Popup de avaliação
    vinhoParaAvaliar?.let { vinho ->
        AvaliacaoDialog(
            vinho = vinho,
            idCliente = idCliente,
            onDismiss = { vinhoParaAvaliar = null },
            onSucesso = { pontos ->
                vinhoParaAvaliar = null
                snackMessage = "Avaliação guardada! +$pontos pontos"
                loading = true
                carregarVinhos()
            },
            onErro = { msg ->
                vinhoParaAvaliar = null
                snackMessage = msg
            }
        )
    }

    Scaffold(snackbarHost = { SnackbarHost(hostState = snackbarHostState) }) { padding ->
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(padding)
                .padding(horizontal = 20.dp, vertical = 16.dp)
        ) {
            Text(
                text = "Vinhos",
                style = MaterialTheme.typography.headlineLarge,
                color = MaterialTheme.colorScheme.onBackground
            )
            Text(
                text = "Os vinhos da Quinta da Azenha",
                style = MaterialTheme.typography.bodyMedium,
                color = MaterialTheme.colorScheme.onSurfaceVariant,
                modifier = Modifier.padding(top = 4.dp, bottom = 20.dp)
            )

            when {
                loading -> {
                    Spacer(modifier = Modifier.height(60.dp))
                    CircularProgressIndicator(
                        modifier = Modifier.align(Alignment.CenterHorizontally),
                        color = MaterialTheme.colorScheme.primary
                    )
                }
                erro.isNotEmpty() -> Text(
                    "Erro: $erro",
                    color = MaterialTheme.colorScheme.error,
                    style = MaterialTheme.typography.bodyMedium
                )
                else -> LazyColumn(verticalArrangement = Arrangement.spacedBy(12.dp)) {
                    items(vinhos) { vinho ->
                        CardVinho(
                            vinho = vinho,
                            onAvaliar = { vinhoParaAvaliar = it }
                        )
                    }
                }
            }
        }
    }
}

@Composable
private fun CardVinho(vinho: Vinho, onAvaliar: (Vinho) -> Unit) {
    Card(
        modifier = Modifier.fillMaxWidth(),
        shape = MaterialTheme.shapes.small,
        colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
    ) {
        Row(
            modifier = Modifier.fillMaxWidth().padding(16.dp),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.Top
        ) {
            Column(modifier = Modifier.weight(1f)) {
                Text(
                    text = vinho.nome,
                    style = MaterialTheme.typography.titleLarge,
                    color = MaterialTheme.colorScheme.onBackground,
                    maxLines = 1,
                    overflow = TextOverflow.Ellipsis
                )
                vinho.casta?.let {
                    Spacer(modifier = Modifier.height(4.dp))
                    Text(it, style = MaterialTheme.typography.labelMedium,
                        color = MaterialTheme.colorScheme.primary)
                }
                vinho.tipo?.let {
                    Spacer(modifier = Modifier.height(4.dp))
                    Text(it, style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurfaceVariant)
                }
                Spacer(modifier = Modifier.height(8.dp))
                Row(verticalAlignment = Alignment.CenterVertically) {
                    Text(
                        text = String.format("%.2f€", vinho.preco),
                        style = MaterialTheme.typography.titleMedium,
                        color = MaterialTheme.colorScheme.primary
                    )
                    vinho.ano?.let {
                        Spacer(modifier = Modifier.width(12.dp))
                        Text("$it", style = MaterialTheme.typography.bodySmall,
                            color = MaterialTheme.colorScheme.onSurfaceVariant)
                    }
                }
                if (vinho.total_avaliacoes > 0) {
                    Spacer(modifier = Modifier.height(4.dp))
                    Text(
                        text = "★ ${"%.1f".format(vinho.media_estrelas)} (${vinho.total_avaliacoes})",
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                }
            }

            IconButton(
                onClick = { if (!vinho.ja_avaliou) onAvaliar(vinho) },
                enabled = !vinho.ja_avaliou
            ) {
                Icon(
                    imageVector = if (vinho.ja_avaliou) Icons.Filled.Star else Icons.Outlined.StarOutline,
                    contentDescription = if (vinho.ja_avaliou) "Já avaliado" else "Avaliar",
                    tint = if (vinho.ja_avaliou)
                        MaterialTheme.colorScheme.primary
                    else
                        MaterialTheme.colorScheme.onSurfaceVariant.copy(alpha = 0.6f)
                )
            }
        }
    }
}

@Composable
private fun AvaliacaoDialog(
    vinho: Vinho,
    idCliente: Int,
    onDismiss: () -> Unit,
    onSucesso: (Int) -> Unit,
    onErro: (String) -> Unit
) {
    var estrelasSeleccionadas by remember { mutableStateOf(0) }
    var comentario by remember { mutableStateOf("") }
    var loading by remember { mutableStateOf(false) }

    Dialog(onDismissRequest = onDismiss) {
        Card(
            shape = MaterialTheme.shapes.medium,
            colors = CardDefaults.cardColors(containerColor = MaterialTheme.colorScheme.surface)
        ) {
            Column(modifier = Modifier.padding(24.dp)) {
                Text(
                    text = vinho.nome,
                    style = MaterialTheme.typography.titleLarge,
                    color = MaterialTheme.colorScheme.onBackground
                )
                Spacer(modifier = Modifier.height(4.dp))
                Text(
                    text = "A sua avaliação",
                    style = MaterialTheme.typography.bodyMedium,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
                Spacer(modifier = Modifier.height(16.dp))

                // Estrelas
                Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                    (1..5).forEach { n ->
                        IconButton(
                            onClick = { estrelasSeleccionadas = n },
                            modifier = Modifier.size(40.dp)
                        ) {
                            Icon(
                                imageVector = if (n <= estrelasSeleccionadas) Icons.Filled.Star else Icons.Outlined.StarOutline,
                                contentDescription = "$n estrelas",
                                tint = if (n <= estrelasSeleccionadas)
                                    MaterialTheme.colorScheme.primary
                                else
                                    MaterialTheme.colorScheme.onSurfaceVariant.copy(alpha = 0.4f),
                                modifier = Modifier.size(32.dp)
                            )
                        }
                    }
                }

                Spacer(modifier = Modifier.height(16.dp))

                OutlinedTextField(
                    value = comentario,
                    onValueChange = { comentario = it },
                    label = { Text("Comentário (opcional)") },
                    modifier = Modifier.fillMaxWidth(),
                    maxLines = 3,
                    colors = OutlinedTextFieldDefaults.colors(
                        focusedBorderColor = MaterialTheme.colorScheme.primary
                    )
                )

                Spacer(modifier = Modifier.height(24.dp))

                Row(
                    modifier = Modifier.fillMaxWidth(),
                    horizontalArrangement = Arrangement.spacedBy(12.dp)
                ) {
                    OutlinedButton(
                        onClick = onDismiss,
                        modifier = Modifier.weight(1f),
                        shape = MaterialTheme.shapes.small
                    ) { Text("Cancelar") }

                    Button(
                        onClick = {
                            if (estrelasSeleccionadas == 0) return@Button
                            if (SessaoManager.obterAvaliacoesSessao() >= 5) {
                                onErro("Limite de 5 avaliações por visita atingido")
                                return@Button
                            }
                            loading = true
                            Thread {
                                var sucesso = false
                                var pontos = 0
                                var erroMsg = ""
                                try {
                                    val client = OkHttpClient()
                                    val body = JSONObject().apply {
                                        put("id_cliente", idCliente)
                                        put("id_vinho", vinho.id_vinho)
                                        put("estrelas", estrelasSeleccionadas)
                                        put("comentario", comentario)
                                    }.toString()
                                    val request = Request.Builder()
                                        .url("${Constants.BASE_URL}/avaliar.ashx")
                                        .post(body.toRequestBody("application/json".toMediaType()))
                                        .header("ngrok-skip-browser-warning", "true")
                                        .build()
                                    val response = client.newCall(request).execute()
                                    val json = JSONObject(response.body?.string() ?: "{}")
                                    if (json.optBoolean("sucesso", false)) {
                                        sucesso = true
                                        pontos = json.optInt("pontos_ganhos", 0)
                                    } else {
                                        erroMsg = json.optString("erro", "Erro ao avaliar")
                                    }
                                } catch (e: Exception) {
                                    erroMsg = "Erro de ligação"
                                } finally {
                                    loading = false
                                    android.os.Handler(android.os.Looper.getMainLooper()).post {
                                        if (sucesso) {
                                            SessaoManager.incrementarAvaliacoesSessao()
                                            onSucesso(pontos)
                                        } else onErro(erroMsg)
                                    }
                                }
                            }.start()
                        },
                        enabled = estrelasSeleccionadas > 0 && !loading,
                        modifier = Modifier.weight(1f),
                        shape = MaterialTheme.shapes.small,
                        colors = ButtonDefaults.buttonColors(
                            containerColor = MaterialTheme.colorScheme.primary
                        )
                    ) {
                        if (loading) CircularProgressIndicator(
                            color = MaterialTheme.colorScheme.onPrimary,
                            strokeWidth = 2.dp,
                            modifier = Modifier.size(20.dp)
                        )
                        else Text("Guardar")
                    }
                }
            }
        }
    }
}