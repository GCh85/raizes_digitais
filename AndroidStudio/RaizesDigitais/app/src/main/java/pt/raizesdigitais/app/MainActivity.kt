package pt.raizesdigitais.app

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Home
import androidx.compose.material.icons.filled.LocationOn
import androidx.compose.material.icons.filled.QrCodeScanner
import androidx.compose.material.icons.outlined.Home
import androidx.compose.material.icons.outlined.LocationOn
import androidx.compose.material.icons.outlined.QrCodeScanner
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.google.gson.Gson
import com.google.gson.reflect.TypeToken
import okhttp3.OkHttpClient
import okhttp3.Request
import pt.raizesdigitais.app.ui.theme.Dourado
import pt.raizesdigitais.app.ui.theme.RaizesDigitaisTheme
import androidx.compose.material.icons.filled.WineBar
import androidx.compose.material.icons.outlined.WineBar

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        SessaoManager.init(applicationContext)
        setContent {
            RaizesDigitaisTheme {
                var temSessao by remember { mutableStateOf(SessaoManager.temSessao()) }

                if (temSessao) {
                    AppNavegacao(onLogout = { temSessao = false })
                } else {
                    LoginScreen(onLoginSucesso = { temSessao = true })
                }
            }
        }
    }
}

@Composable
fun AppNavegacao(onLogout: () -> Unit) {
    var idSeleccionado by remember { mutableStateOf<Int?>(null) }
    var tabActiva by remember { mutableStateOf(0) }
    var qrParaNarrativa by remember { mutableStateOf<Pair<Int, String>?>(null) }

    if (idSeleccionado != null) {
        DetalheScreen(idExperiencia = idSeleccionado!!, onVoltar = { idSeleccionado = null })
        return
    }

    if (qrParaNarrativa != null) {
        IaNarrativaScreen(
            qrId = qrParaNarrativa!!.first,
            tituloQr = qrParaNarrativa!!.second,
            onVoltar = { qrParaNarrativa = null }
        )
        return
    }

    Scaffold(
        containerColor = MaterialTheme.colorScheme.background,
        bottomBar = {
            NavigationBar(
                containerColor = MaterialTheme.colorScheme.surface,
                tonalElevation = 8.dp
            ) {
                NavigationBarItem(
                    selected = tabActiva == 0,
                    onClick = { tabActiva = 0 },
                    icon = {
                        Icon(
                            if (tabActiva == 0) Icons.Filled.Home else Icons.Outlined.Home,
                            contentDescription = "Experiências"
                        )
                    },
                    label = { Text("Experiências") },
                    colors = NavigationBarItemDefaults.colors(
                        selectedIconColor = MaterialTheme.colorScheme.primary,
                        selectedTextColor = MaterialTheme.colorScheme.primary,
                        indicatorColor = MaterialTheme.colorScheme.primaryContainer
                    )
                )
                NavigationBarItem(
                    selected = tabActiva == 1,
                    onClick = { tabActiva = 1 },
                    icon = {
                        Icon(
                            if (tabActiva == 1) Icons.Filled.LocationOn else Icons.Outlined.LocationOn,
                            contentDescription = "Mapa"
                        )
                    },
                    label = { Text("Mapa") },
                    colors = NavigationBarItemDefaults.colors(
                        selectedIconColor = MaterialTheme.colorScheme.primary,
                        selectedTextColor = MaterialTheme.colorScheme.primary,
                        indicatorColor = MaterialTheme.colorScheme.primaryContainer
                    )
                )
                NavigationBarItem(
                    selected = tabActiva == 2,
                    onClick = { tabActiva = 2 },
                    icon = {
                        Icon(
                            if (tabActiva == 2) Icons.Filled.WineBar else Icons.Outlined.WineBar,
                            contentDescription = "Vinhos"
                        )
                    },
                    label = { Text("Vinhos") },
                    colors = NavigationBarItemDefaults.colors(
                        selectedIconColor = MaterialTheme.colorScheme.primary,
                        selectedTextColor = MaterialTheme.colorScheme.primary,
                        indicatorColor = MaterialTheme.colorScheme.primaryContainer
                    )
                )
                NavigationBarItem(
                    selected = tabActiva == 3,
                    onClick = { tabActiva = 3 },
                    icon = {
                        Icon(
                            if (tabActiva == 3) Icons.Filled.QrCodeScanner else Icons.Outlined.QrCodeScanner,
                            contentDescription = "QR Scanner"
                        )
                    },
                    label = { Text("QR Scanner") },
                    colors = NavigationBarItemDefaults.colors(
                        selectedIconColor = MaterialTheme.colorScheme.primary,
                        selectedTextColor = MaterialTheme.colorScheme.primary,
                        indicatorColor = MaterialTheme.colorScheme.primaryContainer
                    )
                )
            }
        }
    ) { innerPadding ->
        Box(modifier = Modifier.padding(innerPadding)) {
            when (tabActiva) {
                0 -> ListaExperienciasScreen(onExperienciaClick = { idSeleccionado = it })
                1 -> MapaScreen()
                2 -> VinhosScreen(idCliente = SessaoManager.obterIdCliente(), onVoltar = { tabActiva = 0 })
                3 -> QrScannerScreen(idCliente = SessaoManager.obterIdCliente(), onQrLido = { id, titulo -> qrParaNarrativa = Pair(id, titulo) })
            }
        }
    }
}

@Composable
fun ListaExperienciasScreen(onExperienciaClick: (Int) -> Unit) {
    var experiencias by remember { mutableStateOf<List<Experiencia>>(emptyList()) }
    var loading by remember { mutableStateOf(true) }
    var erro by remember { mutableStateOf("") }

    LaunchedEffect(Unit) {
        Thread {
            try {
                val client = OkHttpClient()
                val request = Request.Builder()
                    .url("${Constants.BASE_URL}/listar_experiencias.ashx")
                    .header("ngrok-skip-browser-warning", "true")
                    .build()
                val response = client.newCall(request).execute()
                val json = response.body?.string() ?: "[]"
                val tipo = object : TypeToken<List<Experiencia>>() {}.type
                val lista = Gson().fromJson<List<Experiencia>>(json, tipo)
                experiencias = lista
            } catch (e: Exception) {
                erro = e.message ?: "Erro desconhecido"
            } finally {
                loading = false
            }
        }.start()
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(horizontal = 20.dp, vertical = 16.dp)
    ) {
        // Cabeçalho
        Text(
            text = "Experiências",
            style = MaterialTheme.typography.headlineLarge,
            color = MaterialTheme.colorScheme.onBackground
        )
        Text(
            text = "Descubra o que a Quinta tem para si",
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
            else -> LazyColumn(
                verticalArrangement = Arrangement.spacedBy(12.dp)
            ) {
                items(experiencias) { exp ->
                    CardExperiencia(
                        experiencia = exp,
                        onClick = { onExperienciaClick(exp.id_experiencia) }
                    )
                }
            }
        }
    }
}

@Composable
private fun CardExperiencia(
    experiencia: Experiencia,
    onClick: () -> Unit
) {
    Card(
        onClick = onClick,
        modifier = Modifier.fillMaxWidth(),
        shape = MaterialTheme.shapes.small,
        colors = CardDefaults.cardColors(
            containerColor = MaterialTheme.colorScheme.surface
        ),
        elevation = CardDefaults.cardElevation(defaultElevation = 2.dp)
    ) {
        Column(modifier = Modifier.padding(16.dp)) {
            Text(
                text = experiencia.nome,
                style = MaterialTheme.typography.titleLarge,
                color = MaterialTheme.colorScheme.onBackground
            )
            Spacer(modifier = Modifier.height(4.dp))
            experiencia.tipo?.let {
                Text(
                    text = it,
                    style = MaterialTheme.typography.labelMedium,
                    color = MaterialTheme.colorScheme.primary
                )
            }
            Spacer(modifier = Modifier.height(8.dp))
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.SpaceBetween,
                verticalAlignment = Alignment.CenterVertically
            ) {
                Text(
                    text = "${String.format("%.0f", experiencia.preco_por_pessoa)}€ por pessoa",
                    style = MaterialTheme.typography.titleMedium,
                    color = Dourado
                )
                experiencia.duracao_horas?.let { horas ->
                    Text(
                        text = "${String.format("%.1f", horas)}h",
                        style = MaterialTheme.typography.bodySmall,
                        color = MaterialTheme.colorScheme.onSurfaceVariant
                    )
                }
            }
        }
    }
}