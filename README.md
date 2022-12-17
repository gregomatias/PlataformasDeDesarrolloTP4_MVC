TP4 FINAL PLATARORMA DE DESARROLLO:

GUIA:

LoginController:
Se le adiciona seguridad mediante Claims y cookies-
En la vista, el usuario podrá loguear o registrarse.

Home:
Es la pagina de inicio, donde se se muestra el Layout que direcciona a cada opcion del trabajo.
Pasa como parametro el ID de usuario logueado que sera el que interactua en todas las vistas y controllers.


Opciones exclusivas del Admin:
Solo el admin accede  a los indices de: 
Usuario: donde puede realizar el ABM
UsuarioCajaDeAhorro: donde puede asociar una caja a un usuario o desasociar.
CajaDeAhorro: Dentro de CajaDeAhorro se le amplia las posibilidades de accion pudiendo, ademas de deositar y transferir,realizar un ABM de caja.


Pago:
Index: Muestra pagos segun el tipo de usuario.
Create: Si es Admin, se envia una lista de todos los usuarios Caso contrario una Lista solo con un id, el del usuario comun.
Crea un pago que se genera "Pendiente de pago" y pagado = false.
Pagar: Paga un pago creado, si es con tarjeta, agrega un consumo y reduce el limite de consumo. 
Si es con Caja de ahorro, reduce el saldo y genera un movimiento.
En ambos casos actualiza el pago pagado=true y con el metodo elegido.
Eliminar: valida si el pago fue pagado y lo elimina.


PLazo Fijo:
Se hardcodea la tasa en 1.5 dentro del metodo Create
Validaciones en la view con fechaFin minimo 30 días.
Validaciones en la view con fechaIni DateTime.Now ReadOnly
A modo de excepcion para pruebas, el usuario Admin puede setear cualquier fechaIni y Fin.
Por esto mismo no se agrego una Validacion a nivel Backend.
Metodo Pagar: si el plazo fijo esta vencido se transfiere el saldo a la cuenta, se pasa a pagado. Generando el Movimiento correspondiente.
Metodo Delete: Si el plazo fijo esta pagado y vencida la fechaFin se elimina el registro.
Metodo Crear: Crea un plazo fijo validando 30 días como minimo.
Validación de montos negativos a nivel BackEnd en el Modelo  [Range(0, double.MaxValue, ErrorMessage = "Ingrese un valor mayor a 0")]

TarjetaDeCredito:
S puede dar de alta una tarjeta de credito, se genera con autonumeración y un credito hardcodeado de 500000
Este credito se va reduciendo a medida que se le adieran pagos mediante la opción de pagar con tarjeta.
Pagar: paga la tarjeta selleccionando la una caja de ahorro, si esta no tiene saldo no lo permite.
Eliminar: solo permitira la baja de la tarjeta si no tiene consumos adheridos

Caja de Ahorro:
Transferir: Valida y muestra el saldo de la cuenta de origen, la actual seleccionada.
Depositar: Deposita saldo
Retirar, valida el saldo y permite retirar.
Todas las opciones generan movimientos.

Movimiento:
Index: Si el usuario es Admin muestra todos los movimientos de la base - Caso contrario TODOS los movimientos de TODAS las cajas del usuario.
Dentro de este metodo se maneja el filtrado con LINQ Permite filtrar por hasta 3 valores.
Detalle.contain() monto igual y fecha igual.

Salir: Desloguea al usuario.
