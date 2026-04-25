package pt.raizesdigitais.app

data class Disponibilidade(
    val id_disponibilidade: Int,
    val data_hora: String,
    val vagas_total: Int,
    val vagas_disponiveis: Int
)

