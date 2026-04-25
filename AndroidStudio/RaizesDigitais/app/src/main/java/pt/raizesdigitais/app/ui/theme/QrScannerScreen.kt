package pt.raizesdigitais.app

import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import com.google.gson.Gson
import com.journeyapps.barcodescanner.ScanContract
import com.journeyapps.barcodescanner.ScanOptions
import okhttp3.OkHttpClient
import okhttp3.Request
import pt.raizesdigitais.app.ui.theme.Dourado

data class QrConteudo(
    val id_qr: Int,
    val titulo: String,
    val tipo: String,
    val descricao: String?,
    val localizacao: String?,
    val oferta_descricao: String?
)

@Composable
fun QrScannerScreen(idCliente: Int, onQrLido: (Int, String) -> Unit) {
    var erro by remember { mutableStateOf("") }
    var loading by remember { mutableStateOf(false) }

    val scanner = rememberLauncherForActivityResult(ScanContract()) { result ->
        val qrId = result.contents ?: return@rememberLauncherForActivityResult
        loading = true
        erro = ""
        Thread {
            try {
                val client = OkHttpClient()
                
                // 1. Obter conteúdo do QR
                val req = Request.Builder()
                    .url("${Constants.BASE_URL}/qr.ashx?id=$qrId")
                    .header("ngrok-skip-browser-warning", "true")
                    .build()
                val json = client.newCall(req).execute().body?.string() ?: "{}"
                val conteudo = Gson().fromJson(json, QrConteudo::class.java)

                // 2. Registar pontos (30 pts)
                val body = JSONObject().apply {
                    put("id_cliente", idCliente)
                    put("id_qr", conteudo.id_qr)
                }.toString()
                
                val reqPontos = Request.Builder()
                    .url("${Constants.BASE_URL}/qr_pontos.ashx")
                    .post(body.toRequestBody("application/json".toMediaType()))
                    .header("ngrok-skip-browser-warning", "true")
                    .build()
                client.newCall(reqPontos).execute()

                onQrLido(conteudo.id_qr, conteudo.titulo)
            } catch (e: Exception) {
                erro = e.message ?: "Erro"
            } finally {
                loading = false
            }
        }.start()
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(horizontal = 20.dp, vertical = 16.dp),
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        // Cabeçalho
        Text(
            text = "QR Scanner",
            style = MaterialTheme.typography.headlineLarge,
            color = MaterialTheme.colorScheme.onBackground,
            textAlign = TextAlign.Center
        )
        Text(
            text = "Descubra as histórias da Quinta",
            style = MaterialTheme.typography.bodyMedium,
            color = MaterialTheme.colorScheme.onSurfaceVariant,
            modifier = Modifier.padding(top = 4.dp, bottom = 32.dp)
        )

        // Botão principal
        Button(
            onClick = {
                val options = ScanOptions()
                options.setDesiredBarcodeFormats(ScanOptions.QR_CODE)
                options.setPrompt("Aponta a câmara ao QR Code")
                options.setBeepEnabled(true)
                scanner.launch(options)
            },
            modifier = Modifier
                .fillMaxWidth()
                .height(56.dp),
            shape = MaterialTheme.shapes.small,
            colors = ButtonDefaults.buttonColors(
                containerColor = MaterialTheme.colorScheme.primary
            )
        ) {
            Text(
                "Ler QR Code",
                style = MaterialTheme.typography.labelLarge
            )
        }

        Spacer(modifier = Modifier.height(12.dp))

        // Botão de teste (simular QR)
        OutlinedButton(
            onClick = { onQrLido(1, "Vinha de Arinto") },
            modifier = Modifier
                .fillMaxWidth()
                .height(56.dp),
            shape = MaterialTheme.shapes.small,
            colors = ButtonDefaults.outlinedButtonColors(
                contentColor = MaterialTheme.colorScheme.primary
            )
        ) {
            Text(
                "Simular QR (teste)",
                style = MaterialTheme.typography.labelLarge
            )
        }

        // Estados
        when {
            loading -> {
                Spacer(modifier = Modifier.height(32.dp))
                CircularProgressIndicator(color = MaterialTheme.colorScheme.primary)
                Spacer(modifier = Modifier.height(12.dp))
                Text(
                    "A processar...",
                    style = MaterialTheme.typography.bodyMedium,
                    color = MaterialTheme.colorScheme.onSurfaceVariant
                )
            }
            erro.isNotEmpty() -> {
                Spacer(modifier = Modifier.height(32.dp))
                Text(
                    "Erro: $erro",
                    color = MaterialTheme.colorScheme.error,
                    style = MaterialTheme.typography.bodyMedium,
                    textAlign = TextAlign.Center
                )
            }
        }

        // Dica visual
        Spacer(modifier = Modifier.weight(1f))
        Text(
            text = "Os QR Codes estão espalhados pela quinta. Cada um revela uma história única.",
            style = MaterialTheme.typography.bodySmall,
            color = MaterialTheme.colorScheme.onSurfaceVariant,
            textAlign = TextAlign.Center,
            modifier = Modifier.padding(bottom = 16.dp)
        )
    }
}