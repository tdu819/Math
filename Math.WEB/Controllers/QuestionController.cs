﻿using Math.BLL.Abstract.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
// using System.Text.Json;
using System.Text.Json.Serialization;
using Models;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Math.WEB.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuestionController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IQuestionService _questionService;
    private readonly ITopicService _topicService;
    private readonly IAnswerService _answerService;


    public QuestionController(IConfiguration configuration,
        IQuestionService questionService, ITopicService topicService, IAnswerService answerService)
    {
        _configuration = configuration;
        _questionService = questionService;
        _topicService = topicService;
        _answerService = answerService;
    }

    // [HttpGet("Random")]
    // public async Task<ActionResult<ICollection<QuestionModel>>> Get10RandomQuestions()
    // {
    //     try
    //     {
    //         ICollection<QuestionModel> randomQuestions = await _questionService.Get10RandomQuestions();
    //         return Ok(randomQuestions);
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, ex.Message);
    //     }
    // }

    [HttpGet("{topic}")]
    public async Task<ActionResult<ICollection<QuestionModel>>> GetQuestionsByWord(string topic)
    {
        try
        {
            ICollection<QuestionModel> questions;
            // var word = "Arithmetic";
            // Call a service method based on the specified word
            if (topic == "random")
            {
                questions = await _questionService.Get10RandomQuestions();
            }
            else
            {
                questions = await _questionService.GetQuestionsByTopic(topic);
            }

            if (questions == null || questions.Count == 0)
            {
                return NotFound(); // Return 404 Not Found if no questions are found for the word
            }

            return Ok(questions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    // [HttpGet]
    // [Route("GetQuestion")]
    // public async Task<IActionResult> GetQuestion()
    // {
    //     using var client = new HttpClient();
    //
    //     QuestionModel question = _questionService.GetByIdAsync(1).Result;
    //
    //     // Serialize the object using JsonConvert with the PreserveReferencesHandling setting
    //     var serializedQuestion = JsonConvert.SerializeObject(question, new JsonSerializerSettings
    //     {
    //         ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    //     });
    //
    //     // Serialize the object using JsonSerializer with the specified JsonSerializerOptions
    //     var parsedJson = JToken.Parse(serializedQuestion);
    //
    //     // Format the parsed JSON with indented formatting
    //     var prettierJson = parsedJson.ToString(Formatting.Indented);
    //
    //     return Ok(prettierJson);
    // }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<QuestionModel>> GetById(int id)
    {
        try
        {
            var question = await _questionService.GetByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            return Ok(question);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<QuestionModel>> Create(QuestionModel model)
    {
        try
        {
            var createdQuestion = await _questionService.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = createdQuestion.Id }, createdQuestion);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(int id, QuestionModel model)
    {
        try
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var updated = await _questionService.UpdateAsync(model);
            if (updated)
            {
                return NoContent();
            }

            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _questionService.DeleteAsync(id);
            if (deleted)
            {
                return NoContent();
            }

            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
