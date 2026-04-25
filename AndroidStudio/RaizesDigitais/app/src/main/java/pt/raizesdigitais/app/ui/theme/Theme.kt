package pt.raizesdigitais.app.ui.theme

import android.os.Build
import androidx.compose.foundation.isSystemInDarkTheme
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.darkColorScheme
import androidx.compose.material3.lightColorScheme
import androidx.compose.runtime.Composable

private val DarkColorScheme = darkColorScheme(
    primary = VerdeMusgo,
    onPrimary = BrancoMineral,
    primaryContainer = VerdeEscuro,
    onPrimaryContainer = OuroJovem,
    secondary = OuroJovem,
    onSecondary = VerdeEscuro,
    tertiary = Dourado,
    background = VerdeEscuro,
    surface = VerdeEscuro,
    onBackground = BrancoMineral,
    onSurface = BrancoMineral
)

private val LightColorScheme = lightColorScheme(
    primary = VerdeMusgo,
    onPrimary = BrancoMineral,
    primaryContainer = OuroJovem,
    onPrimaryContainer = VerdeEscuro,
    secondary = OuroJovem,
    onSecondary = VerdeEscuro,
    tertiary = Dourado,
    background = BrancoMineral,
    surface = BrancoMineral,
    onBackground = VerdeEscuro,
    onSurface = VerdeEscuro,
    surfaceVariant = NeutroClaro,
    onSurfaceVariant = VerdeTextoLeve,
    outline = Neutro,
    outlineVariant = Neutro
)

@Composable
fun RaizesDigitaisTheme(
    darkTheme: Boolean = isSystemInDarkTheme(),
    dynamicColor: Boolean = false,  // Desativado para manter identidade da marca
    content: @Composable () -> Unit
) {
    val colorScheme = when {
        dynamicColor && Build.VERSION.SDK_INT >= Build.VERSION_CODES.S -> {
            // Por defeito desativado — queremos as cores da marca, não as do sistema
            LightColorScheme
        }
        darkTheme -> DarkColorScheme
        else -> LightColorScheme
    }

    MaterialTheme(
        colorScheme = colorScheme,
        typography = Typography,
        content = content
    )
}