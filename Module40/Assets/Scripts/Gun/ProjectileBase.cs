using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class ProjectileBase : MonoBehaviour
{
    public float timeToDestroy = 2f;
    public float speed = 50f;
    public int damageAmount = 1;
    public List<string> tagsToHit;

    private Vector3 _targetDirection;
    private bool _isBossProjectile = false;
    private bool _directionSet = false;

    // Altura de compensa��o para acertar o player
    public float heightOffset = 1f;

    private void Awake()
    {
        Destroy(gameObject, timeToDestroy);
    }

    private void Start()
    {
        // Se for proj�til do Boss, calcula a dire��o para o player
        if (_isBossProjectile && !_directionSet)
        {
            SetDirectionToPlayer();
        }
    }

    void Update()
    {
        if (_isBossProjectile)
        {
            // Move na dire��o fixa calculada no in�cio
            transform.Translate(_targetDirection * speed * Time.deltaTime, Space.World);
        }
        else
        {
            // Move para frente (comportamento normal)
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    private void SetDirectionToPlayer()
    {
        // Encontra o player na cena
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Calcula a dist�ncia entre o proj�til e o player
            float distance = Vector3.Distance(transform.position, player.transform.position);
            Debug.Log($"Dist�ncia at� o player: {distance} unidades");

            // Pega a posi��o do player e ajusta a altura
            Vector3 targetPosition = player.transform.position;

            // Ajusta a altura do alvo para compensar a diferen�a
            targetPosition.y += heightOffset;

            // Calcula a dire��o do proj�til para o ponto ajustado
            _targetDirection = (targetPosition - transform.position).normalized;
            _directionSet = true;

            // Rotaciona o proj�til para olhar na dire��o do movimento
            RotateTowardsTarget(targetPosition);

            Debug.Log($"Dire��o do proj�til do Boss definida. Altura ajustada: {heightOffset}, Dist�ncia: {distance}");
        }
        else
        {
            Debug.LogWarning("Player n�o encontrado! Usando dire��o padr�o.");
            _targetDirection = transform.forward;
        }
    }

    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        // Calcula a rota��o necess�ria para mirar no alvo
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;

        // Se a dire��o n�o for zero (para evitar erro de normaliza��o)
        if (directionToTarget != Vector3.zero)
        {
            // Cria uma rota��o que olha na dire��o do alvo
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Aplica a rota��o suavemente (opcional - pode ser instant�nea tamb�m)
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f);

            // Ou para rota��o instant�nea:
            // transform.rotation = targetRotation;
        }
    }

    // M�todo p�blico para definir se � um proj�til do boss
    public void SetAsBossProjectile(bool isBossProjectile)
    {
        _isBossProjectile = isBossProjectile;

        // Se for ativado como proj�til do boss e ainda n�o tem dire��o definida
        if (_isBossProjectile && !_directionSet)
        {
            SetDirectionToPlayer();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (var tg in tagsToHit)
        {
            if (collision.transform.tag == tg)
            {
                var damageable = collision.transform.GetComponent<IDamageable>();

                if (damageable != null)
                {
                    Vector3 dir = collision.transform.position - transform.position;
                    dir = -dir.normalized;
                    dir.y = 0;

                    damageable.Damage(damageAmount, dir);
                    Destroy(gameObject);
                }

                break;
            }
        }
    }
}