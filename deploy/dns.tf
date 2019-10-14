
data "aws_route53_zone" "default" {
  name    = "altered-logs.${local.domain_name}."
}

resource "aws_route53_record" "default" {
  zone_id = "${data.aws_route53_zone.default.zone_id}"
  name    = "${var.branch_name}.altered-logs.${local.domain_name}"
  type    = "A"

  alias {
    name                   = "${aws_lb.main.dns_name}"
    zone_id                = "${aws_lb.main.zone_id}"
    evaluate_target_health = "true"
  }
}
