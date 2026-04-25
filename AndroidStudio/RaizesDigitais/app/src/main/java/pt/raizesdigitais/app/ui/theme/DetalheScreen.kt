package pt.raizesdigitais.app

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import com.google.gson.Gson
import com.google.gson.reflect.TypeToken
import okhttp3.FormBody
import okhttp3.OkHttpClient
import okhttp3.Request
import pt.raizesdigitais.app.ui.theme.Dourado

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun DetalheScreen(idExperiencia: Int, onVoltar: () -> Unit) {
    var experiencia by remember { mutableStateOf<Experiencia?>(null) }
    var disponibilidades by remember { mutableStateOf<List<Disponibilidade>>(emptyList()) }
    var slotSeleccionado by remember { mutableStateOf<Disponibilidade?>(null) }
    var numPessoas by remember { mutableStateOf("1") }
    var loading by remember { mutableStateOf(true) }
    var mensagem by remember { mutableStateOf("") }

    LaunchedEffect(idExperiencia) {
        Thread {
            try {
                val client = OkHttpClient()
                val header = "ngrok-skip-browser-warning" to "true"

                // Detalhe
                val reqDetalhe = Request.Builder()
                    .url("${Constants.BASE_URL}/detalhe_experiencia.ashx?id=$idExperiencia")
                    .header(header.first, header.second).build()
                val jsonDetalhe = client.newCall(reqDetalhe).execute().body?.string() ?: "{}"
                experiencia = Gson().fromJson(jsonDetalhe, Experiencia::class.java)

                // Disponibilidade
                val reqDisp = Request.Builder()
                    .url("${Constants.BASE_URL}/disponibilidade.ashx?id=$idExperiencia")
                    .header(header.first, header.second).build()
                val jsonDisp = client.newCall(reqDisp).execute().body?.string() ?: "[]"
                val tipo = object : TypeToken<List<Disponibilidade>>() {}.type
                disponibilidades = Gson().fromJson(jsonDisp, tipo)
            } catch (e: Exception) {
                mensagem = e.message ?: "Erro"
            } finally {
                loading = false
            }
        }.start()
    }

    Column(modifier = Modifier.fillMaxSize()) {
        // Top bar
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

        if (loading) {
            Box(
                modifier = Modifier.fillMaxSize(),
                contentAlignment = Alignment.Center
            ) {
                CircularProgressIndicator(color = MaterialTheme.colorScheme.primary)
            }
        } else {
            experiencia?.let { exp ->
                LazyColumn(
                    modifier = Modifier
                        .fillMaxSize()
                        .padding(horizontal = 20.dp),
                    verticalArrangement = Arrangement.spacedBy(16.dp)
                ) {
                    // Título
                    item {
                        Text(
                            text = exp.nome,
                            style = MaterialTheme.typography.headlineLarge,
                            color = MaterialTheme.colorScheme.onBackground
                        )
                    }

                    // Preço e duração
                    item {
                        Row(
                            horizontalArrangement = Arrangement.spacedBy(16.dp)
                        ) {
                            Text(
                                text = "${String.format("%.0f", exp.preco_por_pessoa)}€/pessoa",
                                style = MaterialTheme.typography.titleMedium,
                                color = Dourado
                            )
                            exp.duracao_horas?.let { horas ->
                                Text(
                                    text = "• ${String.format("%.1f", horas)}h",
                                    style = MaterialTheme.typography.titleMedium,
                                    color = MaterialTheme.colorScheme.onSurfaceVariant
                                )
                            }
                            exp.tipo?.let {
                                Text(
                                    text = "• $it",
                                    style = MaterialTheme.typography.titleMedium,
                                    color = MaterialTheme.colorScheme.onSurfaceVariant
                                )
                            }
                        }
                    }

                    // Descrição
                    item {
                        exp.descricao?.let { desc ->
                            Text(
                                text = desc,
                                style = MaterialTheme.typography.bodyLarge,
                                color = MaterialTheme.colorScheme.onBackground,
                                modifier = Modifier.padding(top = 8.dp)
                            )
                        }
                    }

                    // Capacidade
                    item {
                        Text(
                            text = "Capacidade máxima: ${exp.capacidade_max} pessoas",
                            style = MaterialTheme.typography.bodyMedium,
                            color = MaterialTheme.colorScheme.onSurfaceVariant
                        )
                    }

                    // Datas disponíveis
                    item {
                        Text(
                            text = "Datas disponíveis",
                            style = MaterialTheme.typography.titleMedium,
                            color = MaterialTheme.colorScheme.onBackground,
                            modifier = Modifier.padding(top = 8.dp)
                        )
                    }

                    // Lista de slots
                    items(disponibilidades) { slot ->
                        val seleccionado = slotSeleccionado?.id_disponibilidade == slot.id_disponibilidade
                        Card(
                            onClick = { slotSeleccionado = slot },
                            modifier = Modifier.fillMaxWidth(),
                            shape = MaterialTheme.shapes.small,
                            colors = CardDefaults.cardColors(
                                containerColor = if (seleccionado)
                                    MaterialTheme.colorScheme.primaryContainer
                                else
                                    MaterialTheme.colorScheme.surface
                            ),
                            border = if (seleccionado) null else CardDefaults.outlinedCardBorder()
                        ) {
                            Row(
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .padding(16.dp),
                                horizontalArrangement = Arrangement.SpaceBetween
                            ) {
                                Column {
                                    Text(
                                        text = formatarData(slot.data_hora),
                                        style = MaterialTheme.typography.bodyLarge,
                                        fontWeight = FontWeight.Medium
                                    )
                                    Text(
                                        text = "Vagas: ${slot.vagas_disponiveis}",
                                        style = MaterialTheme.typography.bodySmall,
                                        color = MaterialTheme.colorScheme.onSurfaceVariant
                                    )
                                }
                                if (seleccionado) {
                                    Text(
                                        text = "✓",
                                        style = MaterialTheme.typography.titleLarge,
                                        color = MaterialTheme.colorScheme.primary
                                    )
                                }
                            }
                        }
                    }

                    // Número de pessoas e botão
                    item {
                        Spacer(modifier = Modifier.height(8.dp))

                        OutlinedTextField(
                            value = numPessoas,
                            onValueChange = { if (it.all { c -> c.isDigit() }) numPessoas = it },
                            label = { Text("Nº de pessoas") },
                            modifier = Modifier.fillMaxWidth(),
                            singleLine = true,
                            shape = MaterialTheme.shapes.small
                        )

                        Spacer(modifier = Modifier.height(12.dp))

                        // Resumo do preço
                        if (slotSeleccionado != null) {
                            val pessoas = numPessoas.toIntOrNull() ?: 1
                            val total = exp.preco_por_pessoa * pessoas
                            Text(
                                text = "Total: ${String.format("%.0f", total)}€",
                                style = MaterialTheme.typography.titleMedium,
                                color = Dourado
                            )
                            Spacer(modifier = Modifier.height(12.dp))
                        }

                        Button(
                            onClick = {
                                val slot = slotSeleccionado ?: return@Button
                                val pessoas = numPessoas.toIntOrNull() ?: return@Button
                                val preco = (experiencia?.preco_por_pessoa ?: 0.0) * pessoas
                                Thread {
                                    try {
                                        val client = OkHttpClient()
                                        val body = FormBody.Builder()
                                            .add("id_cliente", "1")
                                            .add("id_disponibilidade", slot.id_disponibilidade.toString())
                                            .add("num_pessoas", pessoas.toString())
                                            .add("preco_total", preco.toString())
                                            .add("notas", "")
                                            .build()
                                        val req = Request.Builder()
                                            .url("${Constants.BASE_URL}/inserir_reserva.ashx")
                                            .header("ngrok-skip-browser-warning", "true")
                                            .post(body).build()
                                        val resp = client.newCall(req).execute().body?.string()
                                        val resultado = Gson().fromJson(resp, Map::class.java)
                                        mensagem = if (resultado["sucesso"] == true)
                                            "Reserva confirmada! Nº ${resultado["num_reserva"]}"
                                        else
                                            "Sem vagas disponíveis."
                                    } catch (e: Exception) {
                                        mensagem = e.message ?: "Erro"
                                    }
                                }.start()
                            },
                            modifier = Modifier
                                .fillMaxWidth()
                                .height(56.dp),
                            shape = MaterialTheme.shapes.small,
                            enabled = slotSeleccionado != null,
                            colors = ButtonDefaults.buttonColors(
                                containerColor = MaterialTheme.colorScheme.primary,
                                disabledContainerColor = MaterialTheme.colorScheme.outline
                            )
                        ) {
                            Text("Reservar", style = MaterialTheme.typography.labelLarge)
                        }

                        if (mensagem.isNotEmpty()) {
                            Spacer(modifier = Modifier.height(16.dp))
                            Card(
                                modifier = Modifier.fillMaxWidth(),
                                colors = CardDefaults.cardColors(
                                    containerColor = if (mensagem.contains("confirmada"))
                                        MaterialTheme.colorScheme.primaryContainer.copy(alpha = 0.3f)
                                    else
                                        MaterialTheme.colorScheme.errorContainer.copy(alpha = 0.3f)
                                ),
                                shape = MaterialTheme.shapes.small
                            ) {
                                Text(
                                    text = mensagem,
                                    style = MaterialTheme.typography.bodyMedium,
                                    color = if (mensagem.contains("confirmada"))
                                        MaterialTheme.colorScheme.primary
                                    else
                                        MaterialTheme.colorScheme.error,
                                    modifier = Modifier.padding(16.dp)
                                )
                            }
                        }

                        Spacer(modifier = Modifier.height(32.dp))
                    }
                }
            }
        }
    }
}

private fun formatarData(dataHora: String): String {
    return try {
        // Formato ISO: 2026-04-12T19:30:00
        val partes = dataHora.split("T")
        if (partes.size == 2) {
            val data = partes[0].split("-")
            val hora = partes[1].substring(0, 5)
            "${data[2]}/${data[1]}/${data[0]} às $hora"
        } else {
            dataHora
        }
    } catch (e: Exception) {
        dataHora
    }
}