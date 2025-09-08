const spec = {
  "openapi": "3.0.4",
  "info": {
    "title": "Camply API",
    "version": "v1"
  },
  "paths": {
    "/api/v1/admin/change-role/{userToChangeId}": {
      "put": {
        "tags": [
          "Admin"
        ],
        "summary": "Change the role of a user.",
        "parameters": [
          {
            "name": "userToChangeId",
            "in": "path",
            "description": "The ID of the user whose role is being changed.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "requestBody": {
          "description": "Contains the new role to assign.",
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/ChangeUserRoleRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/ChangeUserRoleRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/ChangeUserRoleRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Returns success with Camply.Api.Models.Responses.ApiResponse.Success = true",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          },
          "400": {
            "description": "Returns validation failure with Camply.Api.Models.Responses.ApiResponse.Errors"
          },
          "401": {
            "description": "Unauthorized access"
          },
          "403": {
            "description": "Forbidden (user is not admin)"
          }
        }
      }
    },
    "/api/v1/auth/login": {
      "post": {
        "tags": [
          "Auth"
        ],
        "summary": "Logs in a user using username/email and password.",
        "description": "Sample request:\r\n\r\n    POST /api/v1/auth/login\r\n    {\r\n       \"username\": \"user1\",\r\n       \"email\": \"user1@example.com\",\r\n       \"password\": \"password123\"\r\n    }",
        "requestBody": {
          "description": "Login credentials.",
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/LoginRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/LoginRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/LoginRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Login successful, returns Camply.Api.Models.Responses.ApiResponse`1",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/LoginDataApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/LoginDataApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/LoginDataApiResponse"
                }
              }
            }
          },
          "400": {
            "description": "Validation failed, returns Camply.Api.Models.Responses.ApiResponse.Errors"
          },
          "401": {
            "description": "Invalid credentials"
          }
        }
      }
    },
    "/api/v1/auth/register": {
      "post": {
        "tags": [
          "Auth"
        ],
        "summary": "Registers a new user.",
        "description": "Sample request:\r\n\r\n    POST /api/v1/auth/register\r\n    {\r\n       \"name\": \"John\",\r\n       \"surname\": \"Doe\",\r\n       \"username\": \"johndoe\",\r\n       \"email\": \"john@example.com\",\r\n       \"password\": \"password123\",\r\n       \"birthDate\": \"1990-01-01\"\r\n    }",
        "requestBody": {
          "description": "User registration details.",
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/RegisterRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/RegisterRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/RegisterRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Registration successful",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          },
          "400": {
            "description": "Validation failed, returns Camply.Api.Models.Responses.ApiResponse.Errors"
          }
        }
      }
    },
    "/api/v1/auth/logout": {
      "post": {
        "tags": [
          "Auth"
        ],
        "summary": "Logs out the current user.",
        "responses": {
          "200": {
            "description": "Logout successful",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/v1/comment/{postId}": {
      "get": {
        "tags": [
          "Comment"
        ],
        "summary": "Retrieves comments for a specific post.",
        "parameters": [
          {
            "name": "postId",
            "in": "path",
            "description": "ID of the post.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          },
          {
            "name": "numberOfComments",
            "in": "query",
            "description": "Number of comments to retrieve (optional, default 10).",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 10
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Returns the list of comments",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CommentDtoIEnumerableApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommentDtoIEnumerableApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CommentDtoIEnumerableApiResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/v1/comment": {
      "post": {
        "tags": [
          "Comment"
        ],
        "summary": "Creates a new comment on a post.",
        "description": "Sample request:\r\n\r\n    POST /api/v1/comment\r\n    {\r\n       \"content\": \"This is a comment\",\r\n       \"postId\": \"c2b6a0d8-1234-4bfc-8aef-123456789abc\",\r\n       \"parentCommentId\": null\r\n    }",
        "requestBody": {
          "description": "Comment creation data.",
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateCommentRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateCommentRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateCommentRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Comment created successfully",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/v1/comment/{id}": {
      "put": {
        "tags": [
          "Comment"
        ],
        "summary": "Updates an existing comment.",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "description": "ID of the comment to update.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "requestBody": {
          "description": "Updated content of the comment.",
          "content": {
            "application/json": {
              "schema": {
                "type": "string"
              }
            },
            "text/json": {
              "schema": {
                "type": "string"
              }
            },
            "application/*+json": {
              "schema": {
                "type": "string"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Comment updated successfully",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          }
        }
      },
      "delete": {
        "tags": [
          "Comment"
        ],
        "summary": "Deletes a comment.",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "description": "ID of the comment to delete.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Comment deleted successfully",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/v1/forum": {
      "get": {
        "tags": [
          "Forum"
        ],
        "summary": "Get a list of forums with optional filtering.",
        "parameters": [
          {
            "name": "title",
            "in": "query",
            "description": "Optional forum title filter.",
            "schema": {
              "type": "string"
            }
          },
          {
            "name": "tags",
            "in": "query",
            "description": "Optional list of tag IDs to filter by.",
            "schema": {
              "type": "array",
              "items": {
                "type": "string",
                "format": "uuid"
              }
            }
          },
          {
            "name": "skip",
            "in": "query",
            "description": "Number of items to skip (pagination).",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 0
            }
          },
          {
            "name": "take",
            "in": "query",
            "description": "Number of items to take (pagination).",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 20
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Returns a list of forums.",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ForumPrevievDtoIEnumerableApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ForumPrevievDtoIEnumerableApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ForumPrevievDtoIEnumerableApiResponse"
                }
              }
            }
          }
        }
      },
      "post": {
        "tags": [
          "Forum"
        ],
        "summary": "Creates a new forum.",
        "description": "Sample request:\r\n\r\n    POST /api/v1/forum\r\n    {\r\n       \"title\": \"Tech Forum\",\r\n       \"description\": \"A forum about technology\",\r\n       \"tags\": [\"c2b6a0d8-1234-4bfc-8aef-123456789abc\"]\r\n    }",
        "requestBody": {
          "description": "Forum creation data.",
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateForumRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateForumRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateForumRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Forum created successfully.",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          },
          "400": {
            "description": "Validation error."
          }
        }
      }
    },
    "/api/v1/forum/{id}": {
      "get": {
        "tags": [
          "Forum"
        ],
        "summary": "Get a forum by its ID.",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "description": "Forum ID.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Forum found and returned.",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ForumDtoApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ForumDtoApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ForumDtoApiResponse"
                }
              }
            }
          },
          "404": {
            "description": "Forum not found."
          }
        }
      },
      "put": {
        "tags": [
          "Forum"
        ],
        "summary": "Updates an existing forum.",
        "description": "Sample request:\r\n\r\n    PUT /api/v1/forum/{id}\r\n    {\r\n       \"title\": \"Updated Forum\",\r\n       \"description\": \"Updated description\",\r\n       \"tagsId\": [\"c2b6a0d8-1234-4bfc-8aef-123456789abc\"]\r\n    }",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "description": "ID of the forum to update.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "requestBody": {
          "description": "Forum update data.",
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateForumRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateForumRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateForumRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Forum updated successfully.",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          },
          "400": {
            "description": "Validation error."
          },
          "404": {
            "description": "Forum not found."
          }
        }
      },
      "delete": {
        "tags": [
          "Forum"
        ],
        "summary": "Deletes a forum.",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "description": "ID of the forum to delete.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Forum deleted successfully.",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          },
          "404": {
            "description": "Forum not found."
          }
        }
      }
    },
    "/api/v1/post": {
      "get": {
        "tags": [
          "Post"
        ],
        "summary": "Retrieves posts with optional filtering and pagination.",
        "parameters": [
          {
            "name": "forumId",
            "in": "query",
            "description": "Filter by forum ID.",
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          },
          {
            "name": "authorId",
            "in": "query",
            "description": "Filter by author ID.",
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          },
          {
            "name": "title",
            "in": "query",
            "description": "Filter by post title (partial match).",
            "schema": {
              "type": "string"
            }
          },
          {
            "name": "isPinned",
            "in": "query",
            "description": "Filter by pinned status.",
            "schema": {
              "type": "boolean"
            }
          },
          {
            "name": "createdAfter",
            "in": "query",
            "description": "Return posts created after this date.",
            "schema": {
              "type": "string",
              "format": "date-time"
            }
          },
          {
            "name": "createdBefore",
            "in": "query",
            "description": "Return posts created before this date.",
            "schema": {
              "type": "string",
              "format": "date-time"
            }
          },
          {
            "name": "orderBy",
            "in": "query",
            "description": "Field to order by (default: createdDate).",
            "schema": {
              "type": "string",
              "default": "createdDate"
            }
          },
          {
            "name": "descending",
            "in": "query",
            "description": "Whether to sort in descending order (default: true).",
            "schema": {
              "type": "boolean",
              "default": true
            }
          },
          {
            "name": "skip",
            "in": "query",
            "description": "Number of records to skip (for pagination).",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 0
            }
          },
          {
            "name": "take",
            "in": "query",
            "description": "Number of records to take (default: 20).",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 20
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Returns the filtered list of posts",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/PostDtoIEnumerableApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/PostDtoIEnumerableApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/PostDtoIEnumerableApiResponse"
                }
              }
            }
          }
        }
      },
      "post": {
        "tags": [
          "Post"
        ],
        "summary": "Creates a new post in a forum.",
        "description": "Sample request:\r\n\r\n    POST /api/v1/post\r\n    {\r\n       \"title\": \"My first post\",\r\n       \"description\": \"This is the body of the post\",\r\n       \"forumId\": \"a1b2c3d4-5678-90ab-cdef-123456789abc\"\r\n    }",
        "requestBody": {
          "description": "Post creation data.",
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreatePostRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreatePostRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreatePostRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Post created successfully",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/v1/post/{id}": {
      "put": {
        "tags": [
          "Post"
        ],
        "summary": "Updates an existing post.",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "description": "ID of the post to update.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "requestBody": {
          "description": "Updated post data.",
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdatePostRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdatePostRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdatePostRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Post updated successfully",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          }
        }
      },
      "delete": {
        "tags": [
          "Post"
        ],
        "summary": "Deletes a post by ID.",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "description": "ID of the post to delete.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Post deleted successfully",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/v1/post/{id}/pin": {
      "patch": {
        "tags": [
          "Post"
        ],
        "summary": "Pins or unpins a post.",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "description": "ID of the post to pin/unpin.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Post pin status updated successfully",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/BooleanApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/BooleanApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/BooleanApiResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/v1/post/{id}/like": {
      "patch": {
        "tags": [
          "Post"
        ],
        "summary": "Likes or unlikes a post.",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "description": "ID of the post to like/unlike.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Post like status updated successfully",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/BooleanApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/BooleanApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/BooleanApiResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/v1/post/{id}/save": {
      "patch": {
        "tags": [
          "Post"
        ],
        "summary": "Saves or unsaves a post.",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "description": "ID of the post to save/unsave.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Post save status updated successfully",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/BooleanApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/BooleanApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/BooleanApiResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/v1/tag": {
      "get": {
        "tags": [
          "Tag"
        ],
        "summary": "Retrieves all available tags.",
        "responses": {
          "200": {
            "description": "Returns the list of tags",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/TagDtoIEnumerableApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/TagDtoIEnumerableApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/TagDtoIEnumerableApiResponse"
                }
              }
            }
          }
        }
      },
      "post": {
        "tags": [
          "Tag"
        ],
        "summary": "Creates a new tag.",
        "description": "Sample request:\r\n\r\n    POST /api/v1/tag\r\n    \"CSharp\"",
        "requestBody": {
          "description": "Name of the new tag.",
          "content": {
            "application/json": {
              "schema": {
                "type": "string"
              }
            },
            "text/json": {
              "schema": {
                "type": "string"
              }
            },
            "application/*+json": {
              "schema": {
                "type": "string"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Tag created successfully",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/v1/tag/{id}": {
      "put": {
        "tags": [
          "Tag"
        ],
        "summary": "Updates an existing tag.",
        "description": "Sample request:\r\n\r\n    PUT /api/v1/tag/{id}\r\n    \"DotNet\"",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "description": "ID of the tag to update.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "requestBody": {
          "description": "Updated name of the tag.",
          "content": {
            "application/json": {
              "schema": {
                "type": "string"
              }
            },
            "text/json": {
              "schema": {
                "type": "string"
              }
            },
            "application/*+json": {
              "schema": {
                "type": "string"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Tag updated successfully",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          }
        }
      },
      "delete": {
        "tags": [
          "Tag"
        ],
        "summary": "Deletes a tag by ID.",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "description": "ID of the tag to delete.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Tag deleted successfully",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/v1/user/{id}": {
      "get": {
        "tags": [
          "User"
        ],
        "summary": "Retrieves the profile of a specific user by ID.",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "description": "The ID of the user.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Returns the user profile",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/UserProfileDtoApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/UserProfileDtoApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/UserProfileDtoApiResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/v1/user/profile": {
      "get": {
        "tags": [
          "User"
        ],
        "summary": "Retrieves the profile of the currently authenticated user.",
        "responses": {
          "200": {
            "description": "Returns the authenticated user's profile",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/UserProfileDtoApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/UserProfileDtoApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/UserProfileDtoApiResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/v1/user": {
      "put": {
        "tags": [
          "User"
        ],
        "summary": "Updates the profile of the currently authenticated user.",
        "description": "Sample request:\r\n\r\n    PUT /api/v1/user\r\n    {\r\n        \"name\": \"John\",\r\n        \"surname\": \"Doe\",\r\n        \"birthday\": \"1995-05-12\",\r\n        \"username\": \"john_doe\"\r\n    }",
        "requestBody": {
          "description": "The profile update request.",
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateProfileRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateProfileRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateProfileRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Profile updated successfully",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          }
        }
      },
      "delete": {
        "tags": [
          "User"
        ],
        "summary": "Deletes the currently authenticated user's account.",
        "description": "Sample request:\r\n\r\n    DELETE /api/v1/user\r\n    {\r\n        \"password\": \"userPassword123\"\r\n    }",
        "requestBody": {
          "description": "The delete account request (requires password confirmation).",
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteAccountRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteAccountRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/DeleteAccountRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Account deleted successfully",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/v1/user/password": {
      "put": {
        "tags": [
          "User"
        ],
        "summary": "Changes the password of the currently authenticated user.",
        "description": "Sample request:\r\n\r\n    PUT /api/v1/user/password\r\n    {\r\n        \"password\": \"newStrongPassword123\"\r\n    }",
        "requestBody": {
          "description": "The change password request.",
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/ChangePasswordRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/ChangePasswordRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/ChangePasswordRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Password changed successfully",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/v1/vote": {
      "get": {
        "tags": [
          "Vote"
        ],
        "summary": "Retrieves a list of votes with optional filters.",
        "parameters": [
          {
            "name": "forumId",
            "in": "query",
            "description": "Filter by forum ID (optional).",
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          },
          {
            "name": "authorId",
            "in": "query",
            "description": "Filter by author ID (optional).",
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          },
          {
            "name": "voteId",
            "in": "query",
            "description": "Filter by vote ID (optional).",
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          },
          {
            "name": "title",
            "in": "query",
            "description": "Filter by vote title (optional).",
            "schema": {
              "type": "string"
            }
          },
          {
            "name": "skip",
            "in": "query",
            "description": "Number of records to skip for pagination (default = 0).",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 0
            }
          },
          {
            "name": "take",
            "in": "query",
            "description": "Number of records to return (default = 20).",
            "schema": {
              "type": "integer",
              "format": "int32",
              "default": 20
            }
          },
          {
            "name": "orderBy",
            "in": "query",
            "description": "Property name to order results by (optional).",
            "schema": {
              "type": "string"
            }
          },
          {
            "name": "orderDescending",
            "in": "query",
            "description": "Whether to order results descending (default = false).",
            "schema": {
              "type": "boolean",
              "default": false
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/VoteDtoIEnumerableApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/VoteDtoIEnumerableApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/VoteDtoIEnumerableApiResponse"
                }
              }
            }
          }
        }
      },
      "post": {
        "tags": [
          "Vote"
        ],
        "summary": "Creates a new vote in a forum.",
        "requestBody": {
          "description": "Vote creation request containing title, forum ID, and options.",
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateVoteRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateVoteRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateVoteRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/v1/vote/{id}": {
      "put": {
        "tags": [
          "Vote"
        ],
        "summary": "Updates an existing vote.",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "description": "Vote ID.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "requestBody": {
          "description": "Update request containing new title.",
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateVoteRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateVoteRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateVoteRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          }
        }
      },
      "delete": {
        "tags": [
          "Vote"
        ],
        "summary": "Deletes a vote by ID.",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "description": "Vote ID.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/v1/vote/{optionId}/toggle": {
      "patch": {
        "tags": [
          "Vote"
        ],
        "summary": "Toggles a user's vote for a specific option.",
        "parameters": [
          {
            "name": "optionId",
            "in": "path",
            "description": "Vote option ID.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/BooleanApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/BooleanApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/BooleanApiResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/v1/vote/option/{id}": {
      "patch": {
        "tags": [
          "Vote"
        ],
        "summary": "Updates a vote option.",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "description": "Vote option ID.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "requestBody": {
          "description": "Request containing updated name and index.",
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateVoteOptionRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateVoteOptionRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateVoteOptionRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          }
        }
      },
      "delete": {
        "tags": [
          "Vote"
        ],
        "summary": "Deletes a vote option by ID.",
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "description": "Vote option ID.",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ApiResponse"
                }
              }
            }
          }
        }
      }
    }
  },
  "components": {
    "schemas": {
      "ApiResponse": {
        "type": "object",
        "properties": {
          "success": {
            "type": "boolean"
          },
          "message": {
            "type": "string",
            "nullable": true
          },
          "errors": {
            "type": "array",
            "items": {
              "type": "string"
            },
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "BooleanApiResponse": {
        "type": "object",
        "properties": {
          "success": {
            "type": "boolean"
          },
          "message": {
            "type": "string",
            "nullable": true
          },
          "errors": {
            "type": "array",
            "items": {
              "type": "string"
            },
            "nullable": true
          },
          "data": {
            "type": "boolean"
          }
        },
        "additionalProperties": false
      },
      "ChangePasswordRequest": {
        "required": [
          "password"
        ],
        "type": "object",
        "properties": {
          "password": {
            "minLength": 1,
            "type": "string"
          }
        },
        "additionalProperties": false
      },
      "ChangeUserRoleRequest": {
        "type": "object",
        "properties": {
          "role": {
            "$ref": "#/components/schemas/UserRole"
          }
        },
        "additionalProperties": false
      },
      "CommentDto": {
        "type": "object",
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "content": {
            "type": "string",
            "nullable": true
          },
          "authorId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "authorUserName": {
            "type": "string",
            "nullable": true
          },
          "parentId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "isEdited": {
            "type": "boolean"
          },
          "timeExisted": {
            "type": "string",
            "nullable": true
          },
          "replies": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/CommentDto"
            },
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "CommentDtoIEnumerableApiResponse": {
        "type": "object",
        "properties": {
          "success": {
            "type": "boolean"
          },
          "message": {
            "type": "string",
            "nullable": true
          },
          "errors": {
            "type": "array",
            "items": {
              "type": "string"
            },
            "nullable": true
          },
          "data": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/CommentDto"
            },
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "CreateCommentRequest": {
        "required": [
          "content",
          "postId"
        ],
        "type": "object",
        "properties": {
          "content": {
            "maxLength": 200,
            "minLength": 3,
            "type": "string"
          },
          "postId": {
            "type": "string",
            "format": "uuid"
          },
          "parentCommentId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "CreateForumRequest": {
        "required": [
          "tags",
          "title"
        ],
        "type": "object",
        "properties": {
          "title": {
            "maxLength": 100,
            "minLength": 3,
            "type": "string"
          },
          "description": {
            "maxLength": 1000,
            "minLength": 0,
            "type": "string",
            "nullable": true
          },
          "tags": {
            "minItems": 1,
            "type": "array",
            "items": {
              "type": "string",
              "format": "uuid"
            }
          }
        },
        "additionalProperties": false
      },
      "CreatePostRequest": {
        "required": [
          "forumId",
          "title"
        ],
        "type": "object",
        "properties": {
          "title": {
            "minLength": 1,
            "type": "string"
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "forumId": {
            "type": "string",
            "format": "uuid"
          }
        },
        "additionalProperties": false
      },
      "CreateVoteRequest": {
        "required": [
          "forumId",
          "title",
          "voteOptions"
        ],
        "type": "object",
        "properties": {
          "title": {
            "minLength": 1,
            "type": "string"
          },
          "forumId": {
            "type": "string",
            "format": "uuid"
          },
          "voteOptions": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/VoteOptionCreateDto"
            }
          }
        },
        "additionalProperties": false
      },
      "DeleteAccountRequest": {
        "required": [
          "password"
        ],
        "type": "object",
        "properties": {
          "password": {
            "minLength": 1,
            "type": "string"
          }
        },
        "additionalProperties": false
      },
      "ForumDto": {
        "type": "object",
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "title": {
            "type": "string",
            "nullable": true
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "postCount": {
            "type": "integer",
            "format": "int32"
          },
          "timeExisted": {
            "type": "string",
            "nullable": true
          },
          "tags": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/TagDto"
            },
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "ForumDtoApiResponse": {
        "type": "object",
        "properties": {
          "success": {
            "type": "boolean"
          },
          "message": {
            "type": "string",
            "nullable": true
          },
          "errors": {
            "type": "array",
            "items": {
              "type": "string"
            },
            "nullable": true
          },
          "data": {
            "$ref": "#/components/schemas/ForumDto"
          }
        },
        "additionalProperties": false
      },
      "ForumPrevievDto": {
        "type": "object",
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "title": {
            "type": "string",
            "nullable": true
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "tags": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/TagDto"
            },
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "ForumPrevievDtoIEnumerableApiResponse": {
        "type": "object",
        "properties": {
          "success": {
            "type": "boolean"
          },
          "message": {
            "type": "string",
            "nullable": true
          },
          "errors": {
            "type": "array",
            "items": {
              "type": "string"
            },
            "nullable": true
          },
          "data": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/ForumPrevievDto"
            },
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "LoginData": {
        "type": "object",
        "properties": {
          "userId": {
            "type": "string",
            "format": "uuid"
          },
          "token": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "LoginDataApiResponse": {
        "type": "object",
        "properties": {
          "success": {
            "type": "boolean"
          },
          "message": {
            "type": "string",
            "nullable": true
          },
          "errors": {
            "type": "array",
            "items": {
              "type": "string"
            },
            "nullable": true
          },
          "data": {
            "$ref": "#/components/schemas/LoginData"
          }
        },
        "additionalProperties": false
      },
      "LoginRequest": {
        "required": [
          "password"
        ],
        "type": "object",
        "properties": {
          "username": {
            "maxLength": 30,
            "minLength": 0,
            "type": "string",
            "nullable": true
          },
          "email": {
            "type": "string",
            "format": "email",
            "nullable": true
          },
          "password": {
            "minLength": 1,
            "type": "string"
          }
        },
        "additionalProperties": false
      },
      "PostDto": {
        "type": "object",
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "isEdited": {
            "type": "boolean"
          },
          "title": {
            "type": "string",
            "nullable": true
          },
          "content": {
            "type": "string",
            "nullable": true
          },
          "isPinned": {
            "type": "boolean"
          },
          "authorUsername": {
            "type": "string",
            "nullable": true
          },
          "authorId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "likes": {
            "type": "integer",
            "format": "int32"
          },
          "commentsNumber": {
            "type": "integer",
            "format": "int32"
          },
          "existedTime": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "PostDtoIEnumerableApiResponse": {
        "type": "object",
        "properties": {
          "success": {
            "type": "boolean"
          },
          "message": {
            "type": "string",
            "nullable": true
          },
          "errors": {
            "type": "array",
            "items": {
              "type": "string"
            },
            "nullable": true
          },
          "data": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/PostDto"
            },
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "RegisterRequest": {
        "required": [
          "birthDate",
          "email",
          "name",
          "password",
          "surname",
          "username"
        ],
        "type": "object",
        "properties": {
          "name": {
            "maxLength": 50,
            "minLength": 0,
            "type": "string"
          },
          "surname": {
            "maxLength": 50,
            "minLength": 0,
            "type": "string"
          },
          "username": {
            "maxLength": 50,
            "minLength": 5,
            "type": "string"
          },
          "email": {
            "minLength": 1,
            "type": "string",
            "format": "email"
          },
          "password": {
            "maxLength": 100,
            "minLength": 6,
            "type": "string"
          },
          "birthDate": {
            "type": "string",
            "format": "date"
          }
        },
        "additionalProperties": false
      },
      "TagDto": {
        "type": "object",
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "name": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "TagDtoIEnumerableApiResponse": {
        "type": "object",
        "properties": {
          "success": {
            "type": "boolean"
          },
          "message": {
            "type": "string",
            "nullable": true
          },
          "errors": {
            "type": "array",
            "items": {
              "type": "string"
            },
            "nullable": true
          },
          "data": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/TagDto"
            },
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "UpdateForumRequest": {
        "required": [
          "description",
          "tagsId",
          "title"
        ],
        "type": "object",
        "properties": {
          "title": {
            "maxLength": 100,
            "minLength": 3,
            "type": "string"
          },
          "description": {
            "maxLength": 1000,
            "minLength": 5,
            "type": "string"
          },
          "tagsId": {
            "minItems": 1,
            "type": "array",
            "items": {
              "type": "string",
              "format": "uuid"
            }
          }
        },
        "additionalProperties": false
      },
      "UpdatePostRequest": {
        "required": [
          "title"
        ],
        "type": "object",
        "properties": {
          "title": {
            "minLength": 1,
            "type": "string"
          },
          "description": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "UpdateProfileRequest": {
        "required": [
          "birthday",
          "name",
          "surname",
          "username"
        ],
        "type": "object",
        "properties": {
          "name": {
            "minLength": 1,
            "type": "string"
          },
          "surname": {
            "minLength": 1,
            "type": "string"
          },
          "birthday": {
            "type": "string",
            "format": "date-time"
          },
          "username": {
            "minLength": 1,
            "type": "string"
          }
        },
        "additionalProperties": false
      },
      "UpdateVoteOptionRequest": {
        "required": [
          "index",
          "name"
        ],
        "type": "object",
        "properties": {
          "name": {
            "minLength": 1,
            "type": "string"
          },
          "index": {
            "type": "integer",
            "format": "int32"
          }
        },
        "additionalProperties": false
      },
      "UpdateVoteRequest": {
        "required": [
          "title"
        ],
        "type": "object",
        "properties": {
          "title": {
            "minLength": 1,
            "type": "string"
          }
        },
        "additionalProperties": false
      },
      "UserProfileDto": {
        "type": "object",
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "name": {
            "type": "string",
            "nullable": true
          },
          "surname": {
            "type": "string",
            "nullable": true
          },
          "email": {
            "type": "string",
            "nullable": true
          },
          "birthday": {
            "type": "string",
            "format": "date-time"
          },
          "createdPostAmount": {
            "type": "integer",
            "format": "int32"
          },
          "createdForums": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/ForumPrevievDto"
            },
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "UserProfileDtoApiResponse": {
        "type": "object",
        "properties": {
          "success": {
            "type": "boolean"
          },
          "message": {
            "type": "string",
            "nullable": true
          },
          "errors": {
            "type": "array",
            "items": {
              "type": "string"
            },
            "nullable": true
          },
          "data": {
            "$ref": "#/components/schemas/UserProfileDto"
          }
        },
        "additionalProperties": false
      },
      "UserRole": {
        "enum": [
          0,
          1
        ],
        "type": "integer",
        "format": "int32"
      },
      "VoteDto": {
        "type": "object",
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "title": {
            "type": "string",
            "nullable": true
          },
          "isEdited": {
            "type": "boolean"
          },
          "forumId": {
            "type": "string",
            "format": "uuid"
          },
          "authorId": {
            "type": "string",
            "format": "uuid"
          },
          "authorUsername": {
            "type": "string",
            "nullable": true
          },
          "voteOptions": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/VoteOptionDto"
            },
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "VoteDtoIEnumerableApiResponse": {
        "type": "object",
        "properties": {
          "success": {
            "type": "boolean"
          },
          "message": {
            "type": "string",
            "nullable": true
          },
          "errors": {
            "type": "array",
            "items": {
              "type": "string"
            },
            "nullable": true
          },
          "data": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/VoteDto"
            },
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "VoteOptionCreateDto": {
        "type": "object",
        "properties": {
          "name": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "VoteOptionDto": {
        "type": "object",
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "name": {
            "type": "string",
            "nullable": true
          },
          "index": {
            "type": "integer",
            "format": "int32"
          },
          "isEdited": {
            "type": "boolean"
          },
          "votePercentage": {
            "type": "number",
            "format": "float"
          },
          "isVoted": {
            "type": "boolean"
          }
        },
        "additionalProperties": false
      }
    },
    "securitySchemes": {
      "Bearer": {
        "type": "http",
        "description": "Enter 'Bearer' [space] and then your valid token.",
        "scheme": "Bearer",
        "bearerFormat": "JWT"
      }
    }
  },
  "security": [
    {
      "Bearer": [ ]
    }
  ]
}

window.onload = function() {
  //<editor-fold desc="Changeable Configuration Block">

  // the following lines will be replaced by docker/configurator, when it runs in a docker-container
  window.ui = SwaggerUIBundle({
    spec: spec,
    dom_id: '#swagger-ui',
    deepLinking: true,
    presets: [
      SwaggerUIBundle.presets.apis,
      SwaggerUIStandalonePreset
    ],
    plugins: [
      SwaggerUIBundle.plugins.DownloadUrl
    ],
    layout: "StandaloneLayout"
  });

  //</editor-fold>
};
