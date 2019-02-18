#include "MyMesh.h"
void MyMesh::Init(void)
{
	m_bBinded = false;
	m_uVertexCount = 0;

	m_VAO = 0;
	m_VBO = 0;

	m_pShaderMngr = ShaderManager::GetInstance();
}
void MyMesh::Release(void)
{
	m_pShaderMngr = nullptr;

	if (m_VBO > 0)
		glDeleteBuffers(1, &m_VBO);

	if (m_VAO > 0)
		glDeleteVertexArrays(1, &m_VAO);

	m_lVertex.clear();
	m_lVertexPos.clear();
	m_lVertexCol.clear();
}
MyMesh::MyMesh()
{
	Init();
}
MyMesh::~MyMesh() { Release(); }
MyMesh::MyMesh(MyMesh& other)
{
	m_bBinded = other.m_bBinded;

	m_pShaderMngr = other.m_pShaderMngr;

	m_uVertexCount = other.m_uVertexCount;

	m_VAO = other.m_VAO;
	m_VBO = other.m_VBO;
}
MyMesh& MyMesh::operator=(MyMesh& other)
{
	if (this != &other)
	{
		Release();
		Init();
		MyMesh temp(other);
		Swap(temp);
	}
	return *this;
}
void MyMesh::Swap(MyMesh& other)
{
	std::swap(m_bBinded, other.m_bBinded);
	std::swap(m_uVertexCount, other.m_uVertexCount);

	std::swap(m_VAO, other.m_VAO);
	std::swap(m_VBO, other.m_VBO);

	std::swap(m_lVertex, other.m_lVertex);
	std::swap(m_lVertexPos, other.m_lVertexPos);
	std::swap(m_lVertexCol, other.m_lVertexCol);

	std::swap(m_pShaderMngr, other.m_pShaderMngr);
}
void MyMesh::CompleteMesh(vector3 a_v3Color)
{
	uint uColorCount = m_lVertexCol.size();
	for (uint i = uColorCount; i < m_uVertexCount; ++i)
	{
		m_lVertexCol.push_back(a_v3Color);
	}
}
void MyMesh::AddVertexPosition(vector3 a_v3Input)
{
	m_lVertexPos.push_back(a_v3Input);
	m_uVertexCount = m_lVertexPos.size();
}
void MyMesh::AddVertexColor(vector3 a_v3Input)
{
	m_lVertexCol.push_back(a_v3Input);
}
void MyMesh::CompileOpenGL3X(void)
{
	if (m_bBinded)
		return;

	if (m_uVertexCount == 0)
		return;

	CompleteMesh();

	for (uint i = 0; i < m_uVertexCount; i++)
	{
		//Position
		m_lVertex.push_back(m_lVertexPos[i]);
		//Color
		m_lVertex.push_back(m_lVertexCol[i]);
	}
	glGenVertexArrays(1, &m_VAO);//Generate vertex array object
	glGenBuffers(1, &m_VBO);//Generate Vertex Buffered Object

	glBindVertexArray(m_VAO);//Bind the VAO
	glBindBuffer(GL_ARRAY_BUFFER, m_VBO);//Bind the VBO
	glBufferData(GL_ARRAY_BUFFER, m_uVertexCount * 2 * sizeof(vector3), &m_lVertex[0], GL_STATIC_DRAW);//Generate space for the VBO

	// Position attribute
	glEnableVertexAttribArray(0);
	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 2 * sizeof(vector3), (GLvoid*)0);

	// Color attribute
	glEnableVertexAttribArray(1);
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 2 * sizeof(vector3), (GLvoid*)(1 * sizeof(vector3)));

	m_bBinded = true;

	glBindVertexArray(0); // Unbind VAO
}
void MyMesh::Render(matrix4 a_mProjection, matrix4 a_mView, matrix4 a_mModel)
{
	// Use the buffer and shader
	GLuint nShader = m_pShaderMngr->GetShaderID("Basic");
	glUseProgram(nShader); 

	//Bind the VAO of this object
	glBindVertexArray(m_VAO);

	// Get the GPU variables by their name and hook them to CPU variables
	GLuint MVP = glGetUniformLocation(nShader, "MVP");
	GLuint wire = glGetUniformLocation(nShader, "wire");

	//Final Projection of the Camera
	matrix4 m4MVP = a_mProjection * a_mView * a_mModel;
	glUniformMatrix4fv(MVP, 1, GL_FALSE, glm::value_ptr(m4MVP));
	
	//Solid
	glUniform3f(wire, -1.0f, -1.0f, -1.0f);
	glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
	glDrawArrays(GL_TRIANGLES, 0, m_uVertexCount);  

	//Wire
	glUniform3f(wire, 1.0f, 0.0f, 1.0f);
	glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
	glEnable(GL_POLYGON_OFFSET_LINE);
	glPolygonOffset(-1.f, -1.f);
	glDrawArrays(GL_TRIANGLES, 0, m_uVertexCount);
	glDisable(GL_POLYGON_OFFSET_LINE);

	glBindVertexArray(0);// Unbind VAO so it does not get in the way of other objects
}
void MyMesh::AddTri(vector3 a_vBottomLeft, vector3 a_vBottomRight, vector3 a_vTopLeft)
{
	//C
	//| \
	//A--B
	//This will make the triangle A->B->C 
	AddVertexPosition(a_vBottomLeft);
	AddVertexPosition(a_vBottomRight);
	AddVertexPosition(a_vTopLeft);
}
void MyMesh::AddQuad(vector3 a_vBottomLeft, vector3 a_vBottomRight, vector3 a_vTopLeft, vector3 a_vTopRight)
{
	//C--D
	//|  |
	//A--B
	//This will make the triangle A->B->C and then the triangle C->B->D
	AddVertexPosition(a_vBottomLeft);
	AddVertexPosition(a_vBottomRight);
	AddVertexPosition(a_vTopLeft);

	AddVertexPosition(a_vTopLeft);
	AddVertexPosition(a_vBottomRight);
	AddVertexPosition(a_vTopRight);
}
void MyMesh::GenerateCube(float a_fSize, vector3 a_v3Color)
{
	if (a_fSize < 0.01f)
		a_fSize = 0.01f;

	Release();
	Init();

	float fValue = a_fSize * 0.5f;
	//3--2
	//|  |
	//0--1

	vector3 point0(-fValue,-fValue, fValue); //0
	vector3 point1( fValue,-fValue, fValue); //1
	vector3 point2( fValue, fValue, fValue); //2
	vector3 point3(-fValue, fValue, fValue); //3

	vector3 point4(-fValue,-fValue,-fValue); //4
	vector3 point5( fValue,-fValue,-fValue); //5
	vector3 point6( fValue, fValue,-fValue); //6
	vector3 point7(-fValue, fValue,-fValue); //7

	//F
	AddQuad(point0, point1, point3, point2);

	//B
	AddQuad(point5, point4, point6, point7);

	//L
	AddQuad(point4, point0, point7, point3);

	//R
	AddQuad(point1, point5, point2, point6);

	//U
	AddQuad(point3, point2, point7, point6);

	//D
	AddQuad(point4, point5, point0, point1);

	// Adding information about color
	CompleteMesh(a_v3Color);
	CompileOpenGL3X();
}
void MyMesh::GenerateCuboid(vector3 a_v3Dimensions, vector3 a_v3Color)
{
	Release();
	Init();

	vector3 v3Value = a_v3Dimensions * 0.5f;
	//3--2
	//|  |
	//0--1
	vector3 point0(-v3Value.x, -v3Value.y, v3Value.z); //0
	vector3 point1(v3Value.x, -v3Value.y, v3Value.z); //1
	vector3 point2(v3Value.x, v3Value.y, v3Value.z); //2
	vector3 point3(-v3Value.x, v3Value.y, v3Value.z); //3

	vector3 point4(-v3Value.x, -v3Value.y, -v3Value.z); //4
	vector3 point5(v3Value.x, -v3Value.y, -v3Value.z); //5
	vector3 point6(v3Value.x, v3Value.y, -v3Value.z); //6
	vector3 point7(-v3Value.x, v3Value.y, -v3Value.z); //7

	//F
	AddQuad(point0, point1, point3, point2);

	//B
	AddQuad(point5, point4, point6, point7);

	//L
	AddQuad(point4, point0, point7, point3);

	//R
	AddQuad(point1, point5, point2, point6);

	//U
	AddQuad(point3, point2, point7, point6);

	//D
	AddQuad(point4, point5, point0, point1);

	// Adding information about color
	CompleteMesh(a_v3Color);
	CompileOpenGL3X();
}
void MyMesh::GenerateCone(float a_fRadius, float a_fHeight, int a_nSubdivisions, vector3 a_v3Color)
{
	if (a_fRadius < 0.01f)
		a_fRadius = 0.01f;

	if (a_fHeight < 0.01f)
		a_fHeight = 0.01f;

	if (a_nSubdivisions < 3)
		a_nSubdivisions = 3;
	if (a_nSubdivisions > 360)
		a_nSubdivisions = 360;

	Release();
	Init();

	vector3 point1 = vector3(0.0f, 0.0f, 0.0f);
	vector3 point2;
	vector3 point3;
	vector3 point4 = vector3(point1.x, point1.y, point1.z + a_fHeight);//tip of cone
	float step = 2.0*PI / a_nSubdivisions;
	//
	for (float angle = 0.0f; angle < 2.0f*PI; angle += step)
	{
		point2.x = point1.x + (a_fRadius*sinf(angle));
		point2.y = point1.y + (a_fRadius * cosf(angle));
		angle += step;
		point3.x = point1.x + (a_fRadius * sinf(angle));
		point3.y = point1.y + (a_fRadius * cosf(angle));
		angle -= step;
		AddTri(point2, point3, point1);
	}

	//loop for cone tip
	for (float angle = 0.0f; angle < 2.0f*PI; angle += step)
	{
		point2.x = point4.x + (a_fRadius*sinf(angle));
		point2.y = point4.y + (a_fRadius * cosf(angle));
		angle += step;
		point3.x = point4.x + (a_fRadius * sinf(angle));
		point3.y = point4.y + (a_fRadius * cosf(angle));
		angle -= step;
		AddTri(point3, point2, point4);
	}

	// Adding information about color
	CompleteMesh(a_v3Color);
	CompileOpenGL3X();
}
void MyMesh::GenerateCylinder(float a_fRadius, float a_fHeight, int a_nSubdivisions, vector3 a_v3Color)
{
	if (a_fRadius < 0.01f)
		a_fRadius = 0.01f;

	if (a_fHeight < 0.01f)
		a_fHeight = 0.01f;

	if (a_nSubdivisions < 3)
		a_nSubdivisions = 3;
	if (a_nSubdivisions > 360)
		a_nSubdivisions = 360;

	Release();
	Init();

	vector3 point1 = vector3(0.0f, 0.0f, 0.0f);
	vector3 point2;
	vector3 point3;
	vector3 point4 = vector3(0.0f, 0.0f, point1.z + a_fHeight);//will be center of other circular base

	float step = 2.0f*PI / a_nSubdivisions;

	//loop for first circle base
	for (float angle = 0.0f; angle < 2.0f*PI; angle += step)
	{
		point2.x = point1.x + (a_fRadius*sinf(angle));
		point2.y = point1.y + (a_fRadius * cosf(angle));
		angle += step;
		point3.x = point1.x + (a_fRadius * sinf(angle));
		point3.y = point1.y + (a_fRadius * cosf(angle));
		angle -= step;
		AddTri(point2, point3, point1);
	}

	//loop for second circle base
	for (float angle = 0.0f; angle < 2.0f*PI; angle += step)
	{
		point2.x = point4.x + (a_fRadius*sinf(angle));
		point2.y = point4.y + (a_fRadius * cosf(angle));
		point2.z = point4.z;
		angle += step;
		point3.x = point4.x + (a_fRadius * sinf(angle));
		point3.y = point4.y + (a_fRadius * cosf(angle));
		point3.z = point4.z;
		angle -= step;
		AddTri(point3, point2, point4);
	}

	//loop for sides
	vector3 point5;
	vector3 point6;
	for (float angle = 0.0f; angle < 2.0f*PI; angle += step)
	{
		point2.x = point1.x + (a_fRadius*sinf(angle));
		point2.y = point1.y + (a_fRadius * cosf(angle));
		point2.z = point1.z;
		point5 = point2;
		point5.z = point4.z;
		angle += step;
		point3.x = point1.x + (a_fRadius * sinf(angle));
		point3.y = point1.y + (a_fRadius * cosf(angle));
		point3.z = point1.z;
		point6 = point3;
		point6.z = point4.z;
		angle -= step;
		AddQuad(point3, point2, point6, point5);
	}

	// Adding information about color
	CompleteMesh(a_v3Color);
	CompileOpenGL3X();
}
void MyMesh::GenerateTube(float a_fOuterRadius, float a_fInnerRadius, float a_fHeight, int a_nSubdivisions, vector3 a_v3Color)
{
	if (a_fOuterRadius < 0.01f)
		a_fOuterRadius = 0.01f;

	if (a_fInnerRadius < 0.005f)
		a_fInnerRadius = 0.005f;

	if (a_fInnerRadius > a_fOuterRadius)
		std::swap(a_fInnerRadius, a_fOuterRadius);

	if (a_fHeight < 0.01f)
		a_fHeight = 0.01f;

	if (a_nSubdivisions < 3)
		a_nSubdivisions = 3;
	if (a_nSubdivisions > 360)
		a_nSubdivisions = 360;

	Release();
	Init();

	vector3 point1 = vector3(0.0f, 0.0f, 0.0f);
	vector3 point2;
	vector3 point3;
	vector3 point4 = vector3(0.0f, 0.0f, point1.z + a_fHeight);//will be center of other circular base

	float step = 2.0f*PI / a_nSubdivisions;

	//loop for outer sides
	vector3 point5;
	vector3 point6;
	for (float angle = 0.0f; angle < 2.0f*PI; angle += step)
	{
		point2.x = point1.x + (a_fOuterRadius*sinf(angle));
		point2.y = point1.y + (a_fOuterRadius * cosf(angle));
		point2.z = point1.z;
		point5 = point2;
		point5.z = point4.z;
		angle += step;
		point3.x = point1.x + (a_fOuterRadius * sinf(angle));
		point3.y = point1.y + (a_fOuterRadius * cosf(angle));
		point3.z = point1.z;
		point6 = point3;
		point6.z = point4.z;
		angle -= step;
		AddQuad(point3, point2, point6, point5);
	}

	//loop for inner sides
	for (float angle = 0.0f; angle < 2.0f*PI; angle += step)
	{
		point2.x = point1.x + (a_fInnerRadius*sinf(angle));
		point2.y = point1.y + (a_fInnerRadius * cosf(angle));
		point2.z = point1.z;
		point5 = point2;
		point5.z = point4.z;
		angle += step;
		point3.x = point1.x + (a_fInnerRadius * sinf(angle));
		point3.y = point1.y + (a_fInnerRadius * cosf(angle));
		point3.z = point1.z;
		point6 = point3;
		point6.z = point4.z;
		angle -= step;
		AddQuad(point2, point3, point5, point6);
	}

	//loop for first circle base
	for (float angle = 0.0f; angle < 2.0f*PI; angle += step)
	{
		point1.x = a_fInnerRadius * sinf(angle);
		point1.y = a_fInnerRadius * cosf(angle);
		point2.x = point1.x + ((a_fOuterRadius-a_fInnerRadius)*sinf(angle));
		point2.y = point1.y + ((a_fOuterRadius - a_fInnerRadius) * cosf(angle));
		angle += step;
		point5.x = a_fInnerRadius * sinf(angle);
		point5.y = a_fInnerRadius * cosf(angle);
		point5.z = point1.z;
		point3.x = point5.x + ((a_fOuterRadius - a_fInnerRadius) * sinf(angle));
		point3.y = point5.y + ((a_fOuterRadius - a_fInnerRadius) * cosf(angle));
		angle -= step;
		AddQuad(point2, point3, point1,point5);
	}

	//loop for second circle base
	for (float angle = 0.0f; angle < 2.0f*PI; angle += step)
	{
		point4.x = a_fInnerRadius * sinf(angle);
		point4.y = a_fInnerRadius * cosf(angle);
		point2.x = point4.x + ((a_fOuterRadius - a_fInnerRadius)*sinf(angle));
		point2.y = point4.y + ((a_fOuterRadius - a_fInnerRadius) * cosf(angle));
		point2.z = point4.z;
		angle += step;
		point5.x = a_fInnerRadius * sinf(angle);
		point5.y = a_fInnerRadius * cosf(angle);
		point5.z = point4.z;
		point3.x = point5.x + ((a_fOuterRadius - a_fInnerRadius) * sinf(angle));
		point3.y = point5.y + ((a_fOuterRadius - a_fInnerRadius) * cosf(angle));
		point3.z = point4.z;
		angle -= step;
		AddQuad(point3, point2, point5,point4);
	}

	// Adding information about color
	CompleteMesh(a_v3Color);
	CompileOpenGL3X();
}
void MyMesh::GenerateTorus(float a_fOuterRadius, float a_fInnerRadius, int a_nSubdivisionsA, int a_nSubdivisionsB, vector3 a_v3Color)
{
	if (a_fOuterRadius < 0.01f)
		a_fOuterRadius = 0.01f;

	if (a_fInnerRadius < 0.005f)
		a_fInnerRadius = 0.005f;

	if (a_fInnerRadius > a_fOuterRadius)
		std::swap(a_fInnerRadius, a_fOuterRadius);

	if (a_nSubdivisionsA < 3)
		a_nSubdivisionsA = 3;
	if (a_nSubdivisionsA > 360)
		a_nSubdivisionsA = 360;

	if (a_nSubdivisionsB < 3)
		a_nSubdivisionsB = 3;
	if (a_nSubdivisionsB > 360)
		a_nSubdivisionsB = 360;

	Release();
	Init();

	// Replace this with your code
	GenerateCube(a_fOuterRadius * 2.0f, a_v3Color);
	// -------------------------------

	// Adding information about color
	CompleteMesh(a_v3Color);
	CompileOpenGL3X();
}
void MyMesh::GenerateSphere(float a_fRadius, int a_nSubdivisions, vector3 a_v3Color)
{
	if (a_fRadius < 0.01f)
		a_fRadius = 0.01f;

	//Sets minimum and maximum of subdivisions
	if (a_nSubdivisions < 1)
	{
		GenerateCube(a_fRadius * 2.0f, a_v3Color);
		return;
	}
	if (a_nSubdivisions > 6)
		a_nSubdivisions = 6;

	Release();
	Init();
	float step = 2.0*PI / a_nSubdivisions;
	float deltaPhi = step;
	float deltaTheta = step;
	vector3 point1=vector3(0.0f,0.0f,0.0f);
	vector3 point2;
	vector3 point3;
	vector3 point4=vector3(0.0f,0.0f,a_fRadius);

	std::vector<vector3>spherePoints;//vector storing all generated points
	//generating points on sphere
	for (float phi = 0.0f; phi < PI; phi += deltaPhi)
	{
		for (float theta = 0.0f; theta < 2.0f*PI; theta += deltaTheta)
		{
			//x=rcosUsinT, y=rsinUsinT, z=rcosT
			point1.z = a_fRadius * sin(phi + deltaPhi) * cos(theta + deltaTheta);
			point1.x = a_fRadius * sin(phi + deltaPhi) * sin(theta + deltaTheta);
			point1.y = a_fRadius * cos(phi + deltaPhi);

			point2.z = a_fRadius * sin(phi) * cos(theta + deltaTheta);
			point2.x = a_fRadius * sin(phi) * sin(theta + deltaTheta);
			point2.y = a_fRadius * cos(phi);

			point3.z = a_fRadius * sin(phi) * cos(theta);
			point3.x = a_fRadius * sin(phi) * sin(theta);
			point3.y = a_fRadius * cos(phi);

			point4.z = a_fRadius * sin(phi + deltaPhi) * cos(theta);
			point4.x = a_fRadius * sin(phi + deltaPhi) * sin(theta);
			point4.y = a_fRadius * cos(phi + deltaPhi);

			AddQuad(point4, point1, point3, point2);
		}
	}

	//making quads for the sphere


	// Adding information about color
	CompleteMesh(a_v3Color);
	CompileOpenGL3X();
}