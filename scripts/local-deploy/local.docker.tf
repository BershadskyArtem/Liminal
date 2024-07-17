provider "aws" {
  
  access_key = "test"
  secret_key = "test"
  region     = "us-east-1"

  skip_credentials_validation = true
  skip_requesting_account_id  = true
  skip_metadata_api_check     = true
  s3_use_path_style           = true

  # only required for non virtual hosted-style endpoint use case.
  # https://registry.terraform.io/providers/hashicorp/aws/latest/docs#s3_use_path_style
  # https://docs.docker.com/desktop/networking/#i-want-to-connect-from-a-container-to-a-service-on-the-host
  endpoints {
    s3 = "http://host.docker.internal:4566"
  }
}

resource "aws_s3_bucket" "testing_bucket" {
  bucket = "testing-bucket"
  tags = {
    Name        = "My bucket"
    Environment = "test"
  }
}