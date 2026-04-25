package pt.raizesdigitais.app

data class Experiencia(
    val id_experiencia: Int,
    val nome: String,
    val descricao: String?,
    val tipo: String?,
    val preco_por_pessoa: Double,
    val duracao_horas: Double?,
    val capacidade_max: Int,
    val imagem_url: String?
)

