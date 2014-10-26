﻿using Newtonsoft.Json.Linq;
using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Application;
using Swashbuckle.Dummy;
using Swashbuckle.Dummy.Controllers;
using Swashbuckle.Dummy.SwaggerExtensions;

namespace Swashbuckle.Tests.Swagger
{
    [TestFixture]
    public class CoreTests : HttpMessageHandlerTestFixture<SwaggerDocsHandler>
    {
        private SwaggerDocsConfig _swaggerDocsConfig;

        public CoreTests()
            : base("swagger/docs/{apiVersion}")
        { }

        [SetUp]
        public void SetUp()
        {
            _swaggerDocsConfig = new SwaggerDocsConfig();
            _swaggerDocsConfig.SingleApiVersion("1.0", "Test API");

            Func<HttpRequestMessage, string> hostNameResolver = (req) => req.RequestUri.Host + ":" + req.RequestUri.Port;
            Handler = new SwaggerDocsHandler(hostNameResolver, _swaggerDocsConfig);
        }

        [Test]
        public void It_provides_swagger_version_2_0()
        {
            var swagger = GetContent<JObject>("http://tempuri.org/swagger/docs/1.0");

            // TODO: Spec says this should be a string but UI fails if it's not an integer
            Assert.AreEqual("2", swagger["swagger"].ToString());
        }

        [Test]
        public void It_provides_info_version_and_title()
        {
            var swagger = GetContent<JObject>("http://tempuri.org/swagger/docs/1.0");

            var info = swagger["info"];
            Assert.IsNotNull(info);

            var expected = JObject.FromObject(new
                {
                    version = "1.0",
                    title = "Test API",
                });
            Assert.AreEqual(expected.ToString(), info.ToString());
        }

        [Test]
        public void It_provides_host_and_base_path_if_applicable()
        {
            var swagger = GetContent<JObject>("http://tempuri.org:1234/swagger/docs/1.0");

            var host = swagger["host"];
            Assert.AreEqual("tempuri.org:1234", host.ToString());

            var basePath = swagger["basePath"];
            Assert.IsNull(basePath);

            var schemes = swagger["schemes"];
            Assert.IsNull(schemes);

            // When there is a virtual directory
            Configuration = new HttpConfiguration(new HttpRouteCollection("/foobar"));
            swagger = GetContent<JObject>("http://tempuri.org:1234/swagger/docs/1.0");

            basePath = swagger["basePath"];
            Assert.AreEqual("/foobar", basePath.ToString());
        }

        [Test]
        public void It_provides_a_description_for_each_path_in_the_api()
        {
            AddDefaultRoutesFor(new[] { typeof(ProductsController), typeof(CustomersController) });

            var swagger = GetContent<JObject>("http://tempuri.org/swagger/docs/1.0");
            var paths = swagger["paths"];

            var expected = JObject.FromObject(new Dictionary<string, object>
                {
                    {
                        "/products", new
                        {
                            get = new
                            {
                                tags = new [] { "Products" },
                                operationId = "Products_GetAllByType",
                                consumes = new object[]{},
                                produces = new []{ "application/json", "text/json", "application/xml", "text/xml" },
                                parameters = new []
                                {
                                    new
                                    {
                                        name = "type",
                                        @in = "query",
                                        required = true,
                                        type = "string"
                                    }
                                },
                                responses = new Dictionary<string, object>
                                {
                                    {
                                        "200", new
                                        {
                                            description = "OK",
                                            schema = new
                                            {
                                                items = JObject.Parse("{ $ref: \"#/definitions/Product\" }"),
                                                type = "array"
                                            }
                                        }
                                    }
                                },
                                deprecated = false
                            },
                            post = new
                            {
                                tags = new [] { "Products" },
                                operationId = "Products_Create",
                                consumes = new []{ "application/json", "text/json", "application/xml", "text/xml", "application/x-www-form-urlencoded" },
                                produces = new []{ "application/json", "text/json", "application/xml", "text/xml" },
                                parameters = new []
                                {
                                    new
                                    {
                                        name = "product",
                                        @in = "body",
                                        required = true,
                                        schema = JObject.Parse("{ $ref: \"#/definitions/Product\" }")
                                    }
                                },
                                responses = new Dictionary<string, object>
                                {
                                    {
                                        "200", new
                                        {
                                            description = "OK",
                                            schema = new
                                            {
                                                format = "int32",
                                                type = "integer"
                                            }
                                        }
                                    }
                                },
                                deprecated = false
                            }
                        }
                    },
                    {
                        "/products/{id}", new
                        {
                            get = new
                            {
                                tags = new [] { "Products" },
                                operationId = "Products_GetById",
                                consumes = new object[]{},
                                produces = new []{ "application/json", "text/json", "application/xml", "text/xml" },
                                parameters = new []
                                {
                                    new
                                    {
                                        name = "id",
                                        @in = "path",
                                        required = true,
                                        type = "integer",
                                        format = "int32"
                                    }
                                },
                                responses = new Dictionary<string, object>
                                {
                                    {
                                        "200", new
                                        {
                                            description = "OK",
                                            schema = JObject.Parse("{ $ref: \"#/definitions/Product\" }")
                                        }
                                    }
                                },
                                deprecated = false
                            }
                        }
                    },
                    {
                        "/customers", new
                        {
                            post = new
                            {
                                tags = new [] { "Customers" },
                                operationId = "Customers_Create",
                                consumes = new []{ "application/json", "text/json", "application/xml", "text/xml", "application/x-www-form-urlencoded" },
                                produces = new []{ "application/json", "text/json", "application/xml", "text/xml" },
                                parameters = new []
                                {
                                    new
                                    {
                                        name = "customer",
                                        @in = "body",
                                        required = true,
                                        schema = JObject.Parse("{ $ref: \"#/definitions/Customer\" }")
                                    }
                                },
                                responses = new Dictionary<string, object>
                                {
                                    {
                                        "200", new
                                        {
                                            description = "OK",
                                            schema = new
                                            {
                                                format = "int32",
                                                type = "integer"
                                            }
                                        }
                                    }
                                },
                                deprecated = false
                            }
                        }
                    },
                    {
                        "/customers/{id}", new
                        {
                            put = new
                            {
                                tags = new [] { "Customers" },
                                operationId = "Customers_Update",
                                consumes = new []{ "application/json", "text/json", "application/xml", "text/xml", "application/x-www-form-urlencoded" },
                                produces = new string[]{},
                                parameters = new object []
                                {
                                    new
                                    {
                                        name = "id",
                                        @in = "path",
                                        required = true,
                                        type = "integer",
                                        format = "int32",
                                    },
                                    new
                                    {
                                        name = "customer",
                                        @in = "body",
                                        required = true,
                                        schema = JObject.Parse("{ $ref: \"#/definitions/Customer\" }")
                                    }
                                },
                                responses = new Dictionary<string, object>
                                {
                                    {
                                        "200", new
                                        {
                                            description = "OK",
                                        }
                                    }
                                },
                                deprecated = false
                            },
                            delete = new
                            {
                                tags = new [] { "Customers" },
                                operationId = "Customers_Delete",
                                consumes = new string[]{},
                                produces = new string[]{},
                                parameters = new []
                                {
                                    new
                                    {
                                        name = "id",
                                        @in = "path",
                                        required = true,
                                        type = "integer",
                                        format = "int32"
                                    }
                                },
                                responses = new Dictionary<string, object>
                                {
                                    {
                                        "200", new
                                        {
                                            description = "OK",
                                        }
                                    }
                                },
                                deprecated = false
                            }
                        }
                    }
                });

            Assert.AreEqual(expected.ToString(), paths.ToString());
        }

        [Test]
        public void It_sets_the_deprecated_flag_on_actions_that_are_obsolete()
        {
            AddDefaultRouteFor<ObsoleteActionsController>();

            var swagger = GetContent<JObject>("http://tempuri.org/swagger/docs/1.0");
            var putOp = swagger["paths"]["/obsoleteactions/{id}"]["put"];
            var deleteOp = swagger["paths"]["/obsoleteactions/{id}"]["delete"];

            Assert.IsFalse((bool)putOp["deprecated"]);
            Assert.IsTrue((bool)deleteOp["deprecated"]);
        }

        [Test]
        public void It_exposes_config_to_include_additional_info_properties()
        {
            _swaggerDocsConfig.SingleApiVersion("1.0", "Test API")
                .Description("A test API")
                .TermsOfService("Test terms")
                .Contact(c => c
                    .Name("Joe Test")
                    .Url("http://tempuri.org/contact")
                    .Email("joe.test@tempuri.org"))
                .License(c => c
                    .Name("Test License")
                    .Url("http://tempuri.org/license"));

            var swagger = GetContent<JObject>("http://tempuri.org/swagger/docs/1.0");

            var info = swagger["info"];
            Assert.IsNotNull(info);

            var expected = JObject.FromObject(new
                {
                    version = "1.0",
                    title = "Test API",
                    description = "A test API",
                    termsOfService = "Test terms",
                    contact = new
                    {
                        name = "Joe Test",
                        url = "http://tempuri.org/contact",
                        email = "joe.test@tempuri.org"
                    },
                    license = new
                    {
                        name = "Test License",
                        url = "http://tempuri.org/license"
                    }
                });
            Assert.AreEqual(expected.ToString(), info.ToString());
        }

        [Test]
        public void It_exposes_config_to_explictly_provide_supported_schemes()
        {
            _swaggerDocsConfig.Schemes(new[] { "http", "https" });

            var swagger = GetContent<JObject>("http://tempuri.org/swagger/docs/1.0");
            var schemes = swagger["schemes"];
            var expected = JArray.FromObject(new[] { "http", "https" });

            Assert.AreEqual(expected.ToString(), schemes.ToString());
        }

        [Test]
        public void It_exposes_config_to_post_modify_the_document()
        {
            _swaggerDocsConfig.DocumentFilter<ApplyDocumentVendorExtensions>();

            var swagger = GetContent<JObject>("http://tempuri.org/swagger/docs/1.0");
            var xProp = swagger["x-document"];

            Assert.IsNotNull(xProp);
            Assert.AreEqual("foo", xProp.ToString());
        }

        [Test]
        public void It_exposes_config_to_post_modify_operations()
        {
            AddDefaultRouteFor<ProductsController>();

            _swaggerDocsConfig.OperationFilter<AddDefaultResponse>();

            var swagger = GetContent<JObject>("http://tempuri.org/swagger/docs/1.0");
            var getDefaultResponse = swagger["paths"]["/products"]["get"]["responses"]["default"];
            var postDefaultResponse = swagger["paths"]["/products"]["post"]["responses"]["default"];

            Assert.IsNotNull(getDefaultResponse);
            Assert.IsNotNull(postDefaultResponse);
        }

        [Test]
        public void It_exposes_config_to_describe_multiple_api_versions()
        {
            AddAttributeRoutesFrom(typeof(MultipleApiVersionsController).Assembly);

            _swaggerDocsConfig.MultipleApiVersions(
                (apiDesc, targetApiVersion) => SwaggerConfig.ResolveVersionSupportByRouteConstraint(apiDesc, targetApiVersion),
                (c) =>
                {
                    c.Version("1.0", "Test API V1.0");
                    c.Version("2.0", "Test API V2.0");
                });
            
            // 1.0
            var swagger = GetContent<JObject>("http://tempuri.org/swagger/docs/1.0");
            var info = swagger["info"];
            var expected = JObject.FromObject(new
                {
                    version = "1.0",
                    title = "Test API V1.0",
                });
            Assert.AreEqual(expected.ToString(), info.ToString());
            Assert.IsNotNull(swagger["paths"]["/{apiVersion}/todos"]);
            Assert.IsNull(swagger["paths"]["/{apiVersion}/todos/{id}"]);
            
            // 2.0
            swagger = GetContent<JObject>("http://tempuri.org/swagger/docs/2.0");
            info = swagger["info"];
            expected = JObject.FromObject(new
                {
                    version = "2.0",
                    title = "Test API V2.0",
                });
            Assert.AreEqual(expected.ToString(), info.ToString());
            Assert.IsNotNull(swagger["paths"]["/{apiVersion}/todos"]);
            Assert.IsNotNull(swagger["paths"]["/{apiVersion}/todos/{id}"]);
        }

        [Test]
        public void It_handles_additional_route_parameters()
        {
            // i.e. route params that are not included in the action signature
            AddCustomRouteFor<ProductsController>("{apiVersion}/products");

            var swagger = GetContent<JObject>("http://tempuri.org/swagger/docs/1.0");
            var getParams = swagger["paths"]["/{apiVersion}/products"]["get"]["parameters"];

            var expected = JArray.FromObject(new []
                {
                    new
                    {
                        name = "type",
                        @in = "query",
                        required = true,
                        type = "string"
                    },
                    new
                    {
                        name = "apiVersion",
                        @in = "path",
                        required = true,
                        type = "string"
                    }
                });

            Assert.AreEqual(expected.ToString(), getParams.ToString());
        }

        [Test]
        public void It_handles_attribute_routes()
        {
            AddAttributeRoutesFrom(typeof(AttributeRoutesController).Assembly);

            var swagger = GetContent<JObject>("http://tempuri.org/swagger/docs/1.0");
            var path = swagger["paths"]["/subscriptions/{id}/cancel"];
            Assert.IsNotNull(path);
        }

        [Test]
        public void It_errors_on_unknown_api_version_and_returns_status_not_found()
        {
            var response = Get("http://tempuri.org/swagger/docs/1.1");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void It_errors_on_multiple_actions_with_same_path_and_method()
        {
            AddDefaultRouteFor<UnsupportedActionsController>();

            var swagger = GetContent<JObject>("http://tempuri.org/swagger/docs/1.0");
        }
    }
}