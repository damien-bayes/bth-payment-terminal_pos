<?php
// Reference: http://www.fpdf.org/

require('bayesian/vendors/fpdf/tfpdf.php');
require('bayesian/vendors/PHPMailer/PHPMailerAutoload.php');

class PDF extends tFPDF
{
  function Header()
  {
      $this->Image('https://<DOMAIN_NAME>.com/images/brand128x128.png', 16, 0, 32, 32);

      $this->AddFont('Roboto-Light', '', 'Roboto-Light.ttf', true);
      $this->AddFont('Roboto-Bold', '', 'Roboto-Bold.ttf', true);

      $this->SetFont('Roboto-Bold', '', 12);

      $this->Cell(36);

      $this->Cell(80, 8, 'Электронная инкассация денежных средств', 0, 0, 'L');
      $this->Ln(8);

      $this->Cell(36);

      $this->SetFont('Roboto-Light', '', 8);
      $this->SetTextColor(170, 170, 170);
      $this->Cell(80, 4, 'Integrated Payment Terminal System', 0, 0, 'L');
  }
}

$pdf = new PDF();
$pdf->SetMargins(16, 9, 0);
$pdf->AliasNbPages();
$pdf->AddPage();

$pdf->SetTitle('Электронная квитанция об оплате', true);

$pdf->Ln(24);

$pdf->SetFont('Roboto-Light', '', 14);

$pdf->SetFillColor(123, 0, 0);
$pdf->SetTextColor(255, 255, 255);
$pdf->Cell(96, 10, 'Инкассация произведена успешно', 0, 0, 'C', true);

$pdf->SetTextColor(0, 0, 0);

$pdf->Ln(10);
$pdf->SetFillColor(240, 240, 240);
$pdf->SetFont('Roboto-Bold', '', 10);
$pdf->Cell(96, 8, '№ инкассации', 0, 0, 'L', true);
$pdf->Ln(8);
$pdf->SetFont('Roboto-Light', '', 10);
$pdf->Cell(96, 8, '14', 0, 0, 'L');

$pdf->Ln(8);
$pdf->SetFont('Roboto-Bold', '', 10);
$pdf->Cell(96, 8, 'ФИО инкассатора', 0, 0, 'L', true);
$pdf->Ln(8);
$pdf->SetFont('Roboto-Light', '', 10);
$pdf->Cell(96, 8, 'Иванов Иван Иванович', 0, 0, 'L');

$pdf->Ln(8);
$pdf->SetFont('Roboto-Bold', '', 10);
$pdf->Cell(96, 8, 'ИИН/БИН инкассатора', 0, 0, 'L', true);
$pdf->Ln(8);
$pdf->SetFont('Roboto-Light', '', 10);
$pdf->Cell(96, 8, '931030350125', 0, 0, 'L');

$pdf->Ln(8);
$pdf->SetFont('Roboto-Bold', '', 10);
$pdf->Cell(96, 8, 'Дата инкассирования', 0, 0, 'L', true);
$pdf->Ln(8);
$pdf->SetFont('Roboto-Light', '', 10);
$pdf->Cell(96, 8, '21.10.2018 07:05:24', 0, 0, 'L');

$pdf->Ln(8);
$pdf->SetFont('Roboto-Bold', '', 10);
$pdf->Cell(96, 8, 'Инкассирован с', 0, 0, 'L', true);
$pdf->Ln(8);
$pdf->SetFont('Roboto-Light', '', 10);
$pdf->Cell(96, 8, 'IPTS - 50', 0, 0, 'L');

$pdf->Ln(8);
$pdf->SetFont('Roboto-Bold', '', 10);
$pdf->Cell(96, 8, 'Сумма', 0, 0, 'C', true);
$pdf->Ln(8);
$pdf->SetFont('Roboto-Light', '', 24);
$pdf->Cell(96, 8, '200 тенге', 0, 0, 'C');

$pdf->SetFont('Roboto-Light', '', 10);

$pdf->SetFillColor(123, 0, 0);
$pdf->SetTextColor(255, 255, 255);
$pdf->Ln(8);
$pdf->Cell(96, 10, 'Обязательно сохраняйте квитанцию.', 0, 0, 'C', true);
$pdf->Ln(10);
$pdf->Cell(96, 10, 'Спасибо за использование наших услуг!', 0, 0, 'C', true);

$pdf->Ln(18);
$pdf->SetFont('Roboto-Light', '', 8);
$pdf->SetTextColor(170, 170, 170);
$pdf->Cell(0, 4, 'IPTS hash data: ?', 0, 0, 'L');

$smtp = array(
	'host'     => '',
	'username' => '',
	'password' => '',
	'port'     => 465,
	'from'     => array(
		'email' => '',
		'show'  => 'IPTS'
	)
);

/*$mail = new PHPMailer;
$mail->isSMTP();
$mail->CharSet = 'UTF-8';
$mail->Host = $smtp['host'];
$mail->SMTPAuth = true;
$mail->Username = $smtp['username'];
$mail->Password = $smtp['password'];
$mail->SMTPSecure = 'ssl';
$mail->Port = $smtp['port'];

$mail->setFrom($smtp['from']['email'], $smtp['from']['show']);

$mail->addAddress('');

$mail->isHTML(true);

$mail->Subject = 'Электронная квитанция об оплате';
$mail->Body    = '<p>Во вложении электронная квитанция об оплате</p>';
//$mail->AltBody = '';

$doc = $pdf->Output('receipt.pdf', 'S');

$mail->AddStringAttachment($doc, 'ipts-receipt-14.pdf', 'base64', 'application/pdf');
$mail->send();*/

$pdf->Output();
?>
